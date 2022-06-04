using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Common;
using Scool.Dtos;
using Scool.FileHandler;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using Scool.Permission;
using Scool.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Scool.ApplicationServices
{
    public class LrReportAppService : BasicCrudAppService<
        LessonsRegister,
        Guid,
        LRReportDto,
        LRReportDto,
        CreateUpdateLRReportDto,
        CreateUpdateLRReportDto
        >, ILrReportAppService
    {
        private readonly IRepository<LessonsRegister, Guid> _leRepo;
        private readonly IRepository<LessonRegisterPhotos, Guid> _lePhotoRepo;
        private readonly IRepository<Class, Guid> _classesRepo;
        private readonly IFileHandler _fileHandler;
        private readonly IGuidGenerator _guidGenerator;

        public LrReportAppService(
            IRepository<LessonsRegister, Guid> leRepo,
            IRepository<LessonRegisterPhotos, Guid> lePhotoRepo,
            IFileHandler fileHandler,
            IGuidGenerator guidGenerator,
            IRepository<Class, Guid> classesRepo) : base(leRepo)
        {
            _leRepo = leRepo;
            _lePhotoRepo = lePhotoRepo;
            _fileHandler = fileHandler;
            _guidGenerator = guidGenerator;
            _classesRepo = classesRepo;
        }

        [Authorize(ReportsPermissions.CreateNewLRReport)]
        public async override Task<LRReportDto> CreateAsync([FromForm] CreateUpdateLRReportDto input)
        {
            // save photo
            var photoUrl = await _fileHandler.SaveFileAsync(input.Photo);

            // save LR Report
            var reportedClass = await _classesRepo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == input.ClassId);

            var report = await _leRepo.InsertAsync(new LessonsRegister
            {
                ClassId = input.ClassId,
                TotalPoint = input.TotalPoint,
                AbsenceNo = input.AbsenceNo,
                CourseId = ActiveCourse.Id.Value,
                TenantId = CurrentTenant.Id,
                ReportedClassDisplayName = reportedClass?.Name
            });


            await CurrentUnitOfWork.SaveChangesAsync();

            await _lePhotoRepo.InsertAsync(new LessonRegisterPhotos
            {
                LessonRegisterId = report.Id,
                Photo = photoUrl,
                TenantId = CurrentTenant.Id,
            });

            var result = ObjectMapper.Map<LessonsRegister, LRReportDto>(report);
            result.AttachedPhotos = new List<string>()
            {
                photoUrl
            };
            return result;
        }

        [Authorize(ReportsPermissions.UpdateLRReport)]
        public async override Task<LRReportDto> UpdateAsync(Guid id, [FromForm] CreateUpdateLRReportDto input)
        {
            var oReport = await _leRepo
                .Include(x => x.AttachedPhotos)
                .FirstOrDefaultAsync(x => x.Id == id);

            oReport.TotalPoint = input.TotalPoint;
            oReport.AbsenceNo = input.AbsenceNo;
            oReport.TenantId = CurrentTenant.Id;

            // save LR Report
            var report = await _leRepo.UpdateAsync(oReport);

            var result = ObjectMapper.Map<LessonsRegister, LRReportDto>(report);

            if (input.Photo != null)
            {
                // delete old photo
                if (oReport.AttachedPhotos.Count > 0)
                {
                    var photo = oReport.AttachedPhotos.FirstOrDefault();
                    _fileHandler.RemoveFile(photo.Photo);
                    await _lePhotoRepo.DeleteAsync(photo.Id);
                }

                // save photo
                var photoUrl = await _fileHandler.SaveFileAsync(input.Photo);
                await _lePhotoRepo.InsertAsync(new LessonRegisterPhotos
                {
                    LessonRegisterId = oReport.Id,
                    Photo = photoUrl,
                    TenantId = CurrentTenant.Id,
                });

                result.AttachedPhotos = new List<string>()
                {
                    photoUrl
                };
            }

            return result;
        }

        [Authorize(ReportsPermissions.LRReportApproval)]
        public async Task PostAcceptAsync(DcpReportAcceptDto input)
        {
            var reports = await _leRepo.Where(x => input.ReportIds.Contains(x.Id)).ToListAsync();
            foreach (var report in reports)
            {
                report.Status = DcpReportStatus.Approved;
            }
            await _leRepo.UpdateManyAsync(reports);
        }

        [Authorize(ReportsPermissions.LRReportApproval)]
        public async Task PostRejectAsync(Guid id)
        {
            var report = await _leRepo.Where(x => x.Id == id).FirstOrDefaultAsync();
            // TODO: 404 exception
            if (report != null)
            {
                report.Status = DcpReportStatus.Rejected;
                await _leRepo.UpdateAsync(report);
            }
        }

        [Authorize(ReportsPermissions.LRReportApproval)]
        public async Task PostCancelAssessAsync(Guid id)
        {
            var report = await _leRepo.Where(x => x.Id == id).FirstOrDefaultAsync();
            // TODO: 404 exception
            if (report != null)
            {
                report.Status = DcpReportStatus.Created;
                await _leRepo.UpdateAsync(report);
            }
        }

        [Authorize(ReportsPermissions.GetLRApprovalHistory)]
        public async Task<PagingModel<LRReportDto>> PostGetReportsForApprovalAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _leRepo.AsNoTracking().Filter(input.Filter);

            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<LRReportDto>();
            }

            var totalCount = await query.CountAsync();

            query = query.OrderByDescending(x => x.CreationTime);

            query = query.Page(pageIndex, pageSize);

            query = query
                .Include(x => x.CreatorAccount)
                .Include(x => x.AttachedPhotos);

            var items = await query.Select(x => ObjectMapper.Map<LessonsRegister, LRReportDto>(x))
                .ToListAsync();

            return new PagingModel<LRReportDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(ReportsPermissions.GetMyLRReport)]
        public async Task<PagingModel<LRReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input)
        {
            
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _leRepo.Filter(input.Filter);

            if (!CurrentAccount.HasAccount)
            {
                return new PagingModel<LRReportDto>();
            }

            query = query.Where(x => x.CreatorId == CurrentAccount.Id.Value);

            var totalCount = await query.CountAsync();

            query = query.OrderByDescending(x => x.CreationTime);

            query = query.Page(pageIndex, pageSize);

            query = query
                .AsNoTracking()
                .Include(x => x.CreatorAccount)
                .Include(x => x.AttachedPhotos)
                .Include(x => x.Class);

            var items = await query.Select(x => ObjectMapper.Map<LessonsRegister, LRReportDto>(x))
                .ToListAsync();


            return new PagingModel<LRReportDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(ReportsPermissions.RemoveLRReport)]
        public async override Task DeleteAsync(Guid id)
        {
            var oReport = await _leRepo
               .AsQueryable()
               .Include(x => x.AttachedPhotos)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (!CurrentAccount.HasAccount || (CurrentAccount.HasAccount && CurrentAccount.Id.Value != oReport.CreatorId))
            {
                return;
            }

            // delete old photo
            if (oReport.AttachedPhotos.Count > 0)
            {
                var photo = oReport.AttachedPhotos.FirstOrDefault();
                _fileHandler.RemoveFile(photo.Photo);
                await _lePhotoRepo.DeleteAsync(photo.Id);
            }

            await _leRepo.DeleteAsync(id);
        }

        public async override Task<LRReportDto> GetAsync(Guid id)
        {
             var report = await _leRepo
                .AsQueryable()
                .Include(x => x.AttachedPhotos)
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.Id == id);


            var result = ObjectMapper.Map<LessonsRegister, LRReportDto>(report);
            
            return result;
        }
    }
}
