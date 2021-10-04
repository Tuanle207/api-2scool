using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Application.Permissions;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using Scool.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Scool.ApplicationServices
{
    public class DcpReportsAppService : BasicCrudAppService<
        DcpReport,
        Guid,
        DcpReportDto,
        DcpReportDto,
        CreateUpdateDcpReportDto,
        CreateUpdateDcpReportDto
    >, IDcpReportsAppService
    {
        private readonly IRepository<DcpReport, Guid> _dcpReportsRepo;
        private readonly IRepository<DcpClassReport, Guid> _dcpClassReportsRepo;
        private readonly IRepository<DcpClassReportItem, Guid> _dcpClassReportItemsRepo;
        private readonly IRepository<DcpStudentReport, Guid> _dcpStudentReportsRepo;
        private readonly IRepository<Student, Guid> _studentsRepo;
        private readonly IRepository<Regulation, Guid> _regulationsRepo;
        private readonly IRepository<AppUser, Guid> _usersRepo;
        private readonly IGuidGenerator _guidGenerator;

        public DcpReportsAppService
        (
            IRepository<DcpReport, Guid> dcpReportsRepo,
            IRepository<DcpClassReport, Guid> dcpClassReportsRepo,
            IRepository<DcpClassReportItem, Guid> dcpClassReportItemsRepo,
            IRepository<DcpStudentReport, Guid> dcpStudentReportsRepo,
            IRepository<Student, Guid> studentsRepo,
            IRepository<Regulation, Guid> regulationsRepo,
            IRepository<AppUser, Guid> usersRepo,
            IGuidGenerator guidGenerator
        ) : base (dcpReportsRepo)
        {
            _dcpReportsRepo = dcpReportsRepo;
            _dcpClassReportsRepo = dcpClassReportsRepo;
            _dcpClassReportItemsRepo = dcpClassReportItemsRepo;
            _dcpStudentReportsRepo = dcpStudentReportsRepo;
            _studentsRepo = studentsRepo;
            _regulationsRepo = regulationsRepo;
            _usersRepo = usersRepo;
            _guidGenerator = guidGenerator;

            DeletePolicyName = ReportsPermissions.RemoveDcpReport;
        }

        [Authorize(ReportsPermissions.CreateNewDcpReport)]
        public async override Task<DcpReportDto> CreateAsync(CreateUpdateDcpReportDto input)
        {
            var report = new DcpReport(_guidGenerator.Create());

            // report on each class
            var clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                var penalty = 0;

                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reg in faults)
                {
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reg.RegulationId;

                    // calc pelnaty
                    var point = await _regulationsRepo
                        .Where(x => x.Id == reg.RegulationId)
                        .Select(x => x.Point)
                        .FirstOrDefaultAsync();
                    penalty += point;

                    // student breaking the regulations
                    var studentIds = reg.RelatedStudentIds;
                    var students = await _studentsRepo.Where(x => studentIds.Contains(x.Id)).ToListAsync();

                    foreach (var stdnt in students)
                    {
                        var studentReported = await _dcpStudentReportsRepo.InsertAsync(new DcpStudentReport
                        {
                            DcpClassReportItemId = dcpClassReportItem.Id,
                            StudentId = stdnt.Id
                        });
                        // add student report to dcpClassReportItem
                        dcpClassReportItem.RelatedStudents.Add(studentReported);
                    }
                    // add dcpClassReportItem report to dcpClassReport
                    dcpClassReport.Faults.Add(
                        await _dcpClassReportItemsRepo.InsertAsync(dcpClassReportItem)
                    );
                }

                // save penalty point for class
                dcpClassReport.PenaltyTotal = penalty;

                // add dcpClassReportItem report to dcpClassReport
                report.DcpClassReports.Add(
                    await _dcpClassReportsRepo.InsertAsync(dcpClassReport)
                );
            }

            var dcpReport = await _dcpReportsRepo.InsertAsync(report);

            return ObjectMapper.Map<DcpReport, DcpReportDto>(dcpReport);
        }

        [Authorize(ReportsPermissions.UpdateDcpReport)]
        public async override Task<DcpReportDto> UpdateAsync(Guid id, CreateUpdateDcpReportDto input)
        {
            // delete old report
            var oldReport = await _dcpReportsRepo.GetAsync(id);

            await CleanDcpReport(id);

            // replace with nkew report with the same ID (like the way we are doing HTTP PUT)
            var report = new DcpReport(id);

            // report on each class
            var clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                var penalty = 0;
                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reg in faults)
                {
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reg.RegulationId;

                    // calc pelnaty
                    var point = await _regulationsRepo
                        .Where(x => x.Id == reg.RegulationId)
                        .Select(x => x.Point)
                        .FirstOrDefaultAsync();
                    penalty += point;

                    // student breaking the regulations
                    var studentIds = reg.RelatedStudentIds;
                    var students = await _studentsRepo.Where(x => studentIds.Contains(x.Id)).ToListAsync();

                    foreach (var stdnt in students)
                    {
                        var studentReported = await _dcpStudentReportsRepo.InsertAsync(new DcpStudentReport
                        {
                            DcpClassReportItemId = dcpClassReportItem.Id,
                            StudentId = stdnt.Id
                        });
                        // add student report to dcpClassReportItem
                        dcpClassReportItem.RelatedStudents.Add(studentReported);
                    }

                    // save penalty point for class
                    dcpClassReport.PenaltyTotal = penalty;

                    // add dcpClassReportItem report to dcpClassReport
                    dcpClassReport.Faults.Add(
                        await _dcpClassReportItemsRepo.InsertAsync(dcpClassReportItem)
                    );
                }
                // add dcpClassReportItem report to dcpClassReport
                report.DcpClassReports.Add(
                    await _dcpClassReportsRepo.InsertAsync(dcpClassReport)
                );
            }

            report.CreationTime = oldReport.CreationTime;

            var dcpReport = await _dcpReportsRepo.InsertAsync(report);

            return ObjectMapper.Map<DcpReport, DcpReportDto>(dcpReport);
        }

        [Authorize(ReportsPermissions.DcpReportApproval)]
        public async Task PostAcceptAsync(DcpReportAcceptDto input)
        {
            var reports = await _dcpReportsRepo.Where(x => input.ReportIds.Contains(x.Id)).ToListAsync();
            foreach (var report in reports)
            {
                report.Status = DcpReportStatus.Approved;
            }
            await _dcpReportsRepo.UpdateManyAsync(reports);
        }

        [Authorize(ReportsPermissions.DcpReportApproval)]
        public async Task PostRejectAsync(Guid id)
        {
            var report = await _dcpReportsRepo.Where(x => x.Id == id).FirstOrDefaultAsync();
            // TODO: 404 exception
            if (report != null)
            {
                report.Status = DcpReportStatus.Rejected;
                await _dcpReportsRepo.UpdateAsync(report);
            }
        }

        [Authorize(ReportsPermissions.DcpReportApproval)]
        public async Task PostCancelAssessAsync(Guid id)
        {
            var report = await _dcpReportsRepo.Where(x => x.Id == id).FirstOrDefaultAsync();
            // TODO: 404 exception
            if (report != null)
            {
                report.Status = DcpReportStatus.Created;
                await _dcpReportsRepo.UpdateAsync(report);
            }
        }

        [Authorize(ReportsPermissions.GetDcpReportDetail)]
        public async override Task<DcpReportDto> GetAsync(Guid id)
        {
            var item = await _dcpReportsRepo
                .Where(x => x.Id == id)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Class)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.RelatedStudents)
                .ThenInclude(e => e.Student)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.Regulation)
                .ThenInclude(e => e.Criteria)
                .Select(x => ObjectMapper.Map<DcpReport, DcpReportDto>(x))
                .FirstOrDefaultAsync();

            // get creators
            var userId = item.CreatorId;
            if (userId != null)
            {
                var creator = await _usersRepo
                   .Where(x => x.Id == userId)
                   .Select(x => ObjectMapper.Map<AppUser, UserForSimpleListDto>(x))
                   .FirstOrDefaultAsync();
                item.Creator = creator;
            }

            return item;
        }

        [Authorize(ReportsPermissions.GetDcpReportApprovalHistory)]
        public async override Task<PagingModel<DcpReportDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _dcpReportsRepo.AsQueryable();

            var startDateFilter = input.Filter.FirstOrDefault(x => x.Key == "StartDate");
            var endDateFilter = input.Filter.FirstOrDefault(x => x.Key == "EndDate");
            if (startDateFilter != null)
            {
                var startDate = DateTime.ParseExact(startDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date >= startDate.Date);
            }
            if (endDateFilter != null)
            {
                var endDate = DateTime.ParseExact(endDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date <= endDate.Date);
            }

            query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
            query = query.Page(pageIndex, pageSize);
            query = query
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Class)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.RelatedStudents)
                .ThenInclude(e => e.Student)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.Regulation);

            

            var items = await query.Select(x => ObjectMapper.Map<DcpReport, DcpReportDto>(x)).ToListAsync();

            //var items = ObjectMapper.Map<List<DcpReport>, List<DcpReportDto>>(await query.ToListAsync());

            // get creators
            var userIds = items.Where(x => x.CreatorId != null).Select(x => x.CreatorId).ToList();
            var creators = await _usersRepo
                .Where(x => userIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<AppUser, UserForSimpleListDto>(x))
                .ToListAsync();

            // attach creator on each item
            foreach (var report in items)
            {
                if (report.CreatorId != null)
                {
                    report.Creator = creators.FirstOrDefault(x => x.Id == (Guid)report.CreatorId);
                }
            }

            var totalCount = await query.CountAsync();

            return new PagingModel<DcpReportDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(ReportsPermissions.UpdateDcpReport)]
        public async Task<CreateUpdateDcpReportDto> GetUpdateAsync(Guid id)
        {
            var item = await _dcpReportsRepo
               .Where(x => x.Id == id)
               .Include(e => e.DcpClassReports)
               .ThenInclude(e => e.Faults)
               .ThenInclude(e => e.RelatedStudents)
               .Select(x => ObjectMapper.Map<DcpReport, CreateUpdateDcpReportDto>(x))
               .FirstOrDefaultAsync();
            return item;
        }

        private async Task CleanDcpReport(Guid id)
        {
            var report = await _dcpReportsRepo
              .Where(x => x.Id == id)
              .Include(e => e.DcpClassReports)
              .ThenInclude(e => e.Faults)
              .ThenInclude(e => e.RelatedStudents)
              .FirstOrDefaultAsync();

            var classReportsIds = report.DcpClassReports.Select(x => x.Id).ToList();
            var classReports = report.DcpClassReports;
            foreach (var classReport in classReports)
            {
                var classReportItemsIds = classReport.Faults.Select(x => x.Id).ToList();
                var classReportItems = classReport.Faults;

                foreach (var item in classReportItems)
                {
                    var studentsIds = item.RelatedStudents.Select(x => x.Id).ToList();

                    // delete student reports
                    await _dcpClassReportItemsRepo.DeleteManyAsync(studentsIds);
                }
                // delete report class items
                await _dcpClassReportItemsRepo.DeleteManyAsync(classReportItemsIds);
            }
            // delete class reports
            await _dcpClassReportsRepo.DeleteManyAsync(classReportsIds);

            // delete report
            await _dcpReportsRepo.DeleteAsync(report.Id);

            // force save changes in the middle of current unit of work
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [Authorize(ReportsPermissions.GetMyDcpReport)]
        public async Task<PagingModel<DcpReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _dcpReportsRepo.AsQueryable();

            var userId = CurrentUser.Id;
            if (userId == null)
            {
                return null;
            }
            query = query.Where(x => x.CreatorId == userId);

            var startDateFilter = input.Filter.FirstOrDefault(x => x.Key == "StartDate");
            if (startDateFilter != null)
            {
                var startDate = DateTime.ParseExact(startDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date >= startDate.Date);
            }
            var endDateFilter = input.Filter.FirstOrDefault(x => x.Key == "EndDate");
            if (endDateFilter != null)
            {
                var endDate = DateTime.ParseExact(endDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date <= endDate.Date);
            }

            query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
            query = query.Page(pageIndex, pageSize);
            query = query
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Class)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.RelatedStudents)
                .ThenInclude(e => e.Student)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.Regulation);

            var items = ObjectMapper.Map<List<DcpReport>, List<DcpReportDto>>(await query.ToListAsync());

            var totalCount = await query.CountAsync();

            return new PagingModel<DcpReportDto>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<PagingModel<DcpReportDto>> PostGetReportsForApprovalAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            var query = _dcpReportsRepo.AsQueryable();

            var statusList = input.Filter.Where(x => x.Key == "Status").Select(x => x.Value).ToList();
            if (statusList.Count > 0)
            {
                query = query.Where(x => statusList.Contains(x.Status));
            }

            var startDateFilter = input.Filter.FirstOrDefault(x => x.Key == "StartDate");
            if (startDateFilter != null)
            {
                var startDate = DateTime.ParseExact(startDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date >= startDate.Date);
            }

            var endDateFilter = input.Filter.FirstOrDefault(x => x.Key == "EndDate");
            if (endDateFilter != null)
            {
                var endDate = DateTime.ParseExact(endDateFilter.Value, "MM/dd/yyyy", null);
                query = query.Where(e => e.CreationTime.Date <= endDate.Date);
            }

            query = query.OrderByDescending(x => x.CreationTime);
            query = query.Page(pageIndex, pageSize);
            query = query
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Class)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.RelatedStudents)
                .ThenInclude(e => e.Student)
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Faults)
                .ThenInclude(e => e.Regulation);

            var items = ObjectMapper.Map<List<DcpReport>, List<DcpReportDto>>(await query.ToListAsync());

            // get creators
            var userIds = items.Where(x => x.CreatorId != null).Select(x => x.CreatorId).ToList();
            var creators = await _usersRepo
                .Where(x => userIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<AppUser, UserForSimpleListDto>(x))
                .ToListAsync();

            // attach creator on each item
            foreach (var report in items)
            {
                if (report.CreatorId != null)
                {
                    report.Creator = creators.FirstOrDefault(x => x.Id == (Guid)report.CreatorId);
                }
            }

            var totalCount = await query.CountAsync();

            return new PagingModel<DcpReportDto>(items, totalCount, pageIndex, pageSize);
        }
    }
}
