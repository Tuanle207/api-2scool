using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Application.Dtos;
using Scool.Application.FileHandler;
using Scool.Application.IApplicationServices;
using Scool.Application.Permissions;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
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
        private readonly ICurrentUser _currentUser;
        private readonly FileHandler _fileHandler;
        private readonly IGuidGenerator _guidGenerator;

        public LrReportAppService(
            IRepository<LessonsRegister, Guid> leRepo,
            IRepository<LessonRegisterPhotos, Guid> lePhotoRepo,
            ICurrentUser currentUser,
            FileHandler fileHandler,
            IGuidGenerator guidGenerator 
            ) : base(leRepo)
        {
            _leRepo = leRepo;
            _lePhotoRepo = lePhotoRepo;
            _currentUser = currentUser;
            _fileHandler = fileHandler;
            _guidGenerator = guidGenerator;
        }

        [Authorize(ReportsPermissions.CreateNewLRReport)]
        public async override Task<LRReportDto> CreateAsync([FromForm] CreateUpdateLRReportDto input)
        {
            // save photo
            var photoUrl = await _fileHandler.SaveFileAsync(input.Photo);

            // save LR Report
            var report = await _leRepo.InsertAsync(new LessonsRegister
            {
                ClassId = input.ClassId,
                TotalPoint = input.TotalPoint,
                AbsenceNo = input.AbsenceNo,
            });

            await CurrentUnitOfWork.SaveChangesAsync();

            await _lePhotoRepo.InsertAsync(new LessonRegisterPhotos
            {
                LessonRegisterId = report.Id,
                Photo = photoUrl
            });

            var result = ObjectMapper.Map<LessonsRegister, LRReportDto>(report);
            result.AttachedPhotos = new List<string>()
            {
                photoUrl
            };
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

            var query = _leRepo.AsQueryable();

            var statusList = input.Filter.Where(x => x.Key == "Status").Select(x => x.Value).ToList();
            if (statusList.Count > 0)
            {
                query = query.Where(x => statusList.Contains(x.Status));
            }


            var totalCount = await query.CountAsync();

            query = query
                 .OrderByDescending(x => x.CreationTime)
                 .Page(pageIndex, pageSize)
                 .Include(x => x.AttachedPhotos)
                 .Include(x => x.Class);

            var items = await query.Select(x => ObjectMapper.Map<LessonsRegister, LRReportDto>(x)).ToListAsync();


            return new PagingModel<LRReportDto>(items, totalCount);
        }

        [Authorize(ReportsPermissions.GetMyLRReport)]
        public async Task<PagingModel<LRReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input)
        {
            var userId = _currentUser.Id;
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _leRepo.AsQueryable()
                .WhereIf(userId.HasValue, x => x.CreatorId == userId.Value);

            var totalCount = await query.CountAsync();

            query = query
                 .OrderByDescending(x => x.CreationTime)
                 .Page(pageIndex, pageSize)
                 .Include(x => x.AttachedPhotos)
                 .Include(x => x.Class);

            var items = await query.Select(x => ObjectMapper.Map<LessonsRegister, LRReportDto>(x)).ToListAsync();


            return new PagingModel<LRReportDto>(items, totalCount);
        }

        [Authorize(ReportsPermissions.UpdateLRReport)]
        public async override Task<LRReportDto> UpdateAsync(Guid id, [FromBody] CreateUpdateLRReportDto input)
        {
            var oReport = _leRepo
                .AsQueryable()
                .Include(x => x.AttachedPhotos)
                .FirstOrDefault(x => x.Id == id);


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
                Photo = photoUrl
            });

            // save LR Report
            var report = await _leRepo.UpdateAsync(new LessonsRegister
            {
                ClassId = input.ClassId,
                TotalPoint = input.TotalPoint,
                AbsenceNo = input.AbsenceNo,
            });

            var result = ObjectMapper.Map<LessonsRegister, LRReportDto>(report);
            result.AttachedPhotos = new List<string>()
            {
                photoUrl
            };
            return result;
        }

        [Authorize(ReportsPermissions.RemoveLRReport)]
        public async override Task DeleteAsync(Guid id)
        {
            var oReport = _leRepo
               .AsQueryable()
               .Include(x => x.AttachedPhotos)
               .FirstOrDefault(x => x.Id == id);

            if (_currentUser.Id.HasValue && _currentUser.Id.Value != oReport.CreatorId)
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


    }
}
