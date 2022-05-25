using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using Scool.Notification;
using Scool.Permission;
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
        private readonly INotificationService _notificationService;

        public DcpReportsAppService
        (
            IRepository<DcpReport, Guid> dcpReportsRepo,
            IRepository<DcpClassReport, Guid> dcpClassReportsRepo,
            IRepository<DcpClassReportItem, Guid> dcpClassReportItemsRepo,
            IRepository<DcpStudentReport, Guid> dcpStudentReportsRepo,
            IRepository<Student, Guid> studentsRepo,
            IRepository<Regulation, Guid> regulationsRepo,
            IRepository<AppUser, Guid> usersRepo,
            IGuidGenerator guidGenerator,
            INotificationService notificationService) : base(dcpReportsRepo)
        {
            _dcpReportsRepo = dcpReportsRepo;
            _dcpClassReportsRepo = dcpClassReportsRepo;
            _dcpClassReportItemsRepo = dcpClassReportItemsRepo;
            _dcpStudentReportsRepo = dcpStudentReportsRepo;
            _studentsRepo = studentsRepo;
            _regulationsRepo = regulationsRepo;
            _usersRepo = usersRepo;
            _guidGenerator = guidGenerator;
            _notificationService = notificationService;

            DeletePolicyName = ReportsPermissions.RemoveDcpReport;
        }

        [Authorize(ReportsPermissions.CreateNewDcpReport)]
        public async override Task<DcpReportDto> CreateAsync(CreateUpdateDcpReportDto input)
        {
            var report = new DcpReport(_guidGenerator.Create());
            report.TenantId = CurrentTenant.Id;

            // report on each class
            IList<CreateUpdateDcpClassReportDto> clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                int penalty = 0;

                dcpClassReport.TenantId = CurrentTenant.Id;
                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reg in faults)
                {
                    IList<Guid> listStudentId = reg.RelatedStudentIds;
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.TenantId = CurrentTenant.Id;
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reg.RegulationId;

                    // accumulate penalty
                    Regulation regulation = await _regulationsRepo
                        .Where(x => x.Id == reg.RegulationId)
                        .FirstOrDefaultAsync();
                    int mutiply = regulation.Type == RegulationType.Class ? 1 : listStudentId.Count;
                    penalty += regulation.Point * mutiply;

                    // student breaking the regulations
                    IList<Student> students = await _studentsRepo.Where(x => listStudentId.Contains(x.Id))
                        .ToListAsync();

                    foreach (var stdnt in students)
                    {
                        var studentReported = await _dcpStudentReportsRepo.InsertAsync(new DcpStudentReport
                        {
                            DcpClassReportItemId = dcpClassReportItem.Id,
                            StudentId = stdnt.Id,
                            TenantId = CurrentTenant.Id
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

            var dcpReport = await _dcpReportsRepo.InsertAsync(report, autoSave: true);

            await _notificationService.CreateNotificationForRoleAsync(NotificationType.DcpReportCreated, AppRole.DcpManager, CurrentAccount.Id);
            await _notificationService.NotifyToRoleAsync(AppRole.DcpManager, NotificationType.DcpReportCreated);

            return ObjectMapper.Map<DcpReport, DcpReportDto>(dcpReport);
        }

        [Authorize(ReportsPermissions.UpdateDcpReport)]
        public async override Task<DcpReportDto> UpdateAsync(Guid id, CreateUpdateDcpReportDto input)
        {
            // delete old report
            var oldReport = await _dcpReportsRepo.GetAsync(id);

            await CleanDcpReport(id);

            // replace with new report with the same ID (like the way we are doing HTTP PUT)
            var report = new DcpReport(id);
            report.TenantId = CurrentTenant.Id;
            // report on each class
            IList<CreateUpdateDcpClassReportDto> clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                int penalty = 0;
                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;
                dcpClassReport.TenantId = CurrentTenant.Id;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reg in faults)
                {
                    IList<Guid> listStudentId = reg.RelatedStudentIds;
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.TenantId = CurrentTenant.Id;
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reg.RegulationId;

                    // accumulate penalty
                    Regulation regulation = await _regulationsRepo
                        .Where(x => x.Id == reg.RegulationId)
                        .FirstOrDefaultAsync();
                    int mutiply = regulation.Type == RegulationType.Class ? 1 : listStudentId.Count;
                    penalty += regulation.Point * mutiply;

                    // student breaking the regulations
                    IList<Student> students = await _studentsRepo.Where(x => listStudentId.Contains(x.Id))
                        .ToListAsync();

                    foreach (var stdnt in students)
                    {
                        var studentReported = await _dcpStudentReportsRepo.InsertAsync(new DcpStudentReport
                        {
                            DcpClassReportItemId = dcpClassReportItem.Id,
                            StudentId = stdnt.Id,
                            TenantId = CurrentTenant.Id
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
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Include(x => x.CreatorAccount)
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

            return item;
        }

        [Obsolete]
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
                .Include(x => x.CreatorAccount)
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

        [Authorize(ReportsPermissions.GetMyDcpReport)]
        public async Task<PagingModel<DcpReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            // Create query with filters
            var query = _dcpReportsRepo.Filter(input.Filter);

            // Filter by current user
            if (!CurrentAccount.HasAccount)
            {
                return new PagingModel<DcpReportDto>();
            }
            query = query.Where(x => x.CreatorId == CurrentAccount.Id.Value);

            // Count total count
            int totalCount = await query.CountAsync();

            // Sort descending by time
            query = query.OrderByDescending(x => x.CreationTime);

            // Pagination
            query = query.Page(pageIndex, pageSize);

            // Include navigation properties
            query = query
                .AsNoTracking()
                .Include(e => e.DcpClassReports)
                .ThenInclude(e => e.Class);

            // Call query with projection
            var items = await query.Select(x => ObjectMapper.Map<DcpReport, DcpReportDto>(x))
                .ToListAsync();

            return new PagingModel<DcpReportDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(ReportsPermissions.GetDcpReportApprovalHistory)]
        public async Task<PagingModel<DcpReportDto>> PostGetReportsForApprovalAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            // Create query with filters
            var query = _dcpReportsRepo.Filter(input.Filter);

            int totalCount = await query.CountAsync();

            // Sort descending by time
            query = query.OrderByDescending(x => x.CreationTime);

            // Pagination
            query = query.Page(pageIndex, pageSize);

            // Include navigation properties
            query = query
                .AsNoTracking()
                .Include(x => x.CreatorAccount)
                .Include(x => x.DcpClassReports)
                    .ThenInclude(x => x.Class);

            // Call query with projection
            var items = await query.Select(x => ObjectMapper.Map<DcpReport, DcpReportDto>(x))
                .ToListAsync();

            return new PagingModel<DcpReportDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(ReportsPermissions.RemoveDcpReport)]
        public override async Task DeleteAsync(Guid id)
        {
            var oReport = await _dcpReportsRepo.FirstOrDefaultAsync(x => x.Id == id);

            if (!CurrentAccount.HasAccount || (CurrentAccount.HasAccount && CurrentAccount.Id.Value != oReport.CreatorId))
            {
                return;
            }
            await _dcpReportsRepo.DeleteAsync(id);
        }

        private async Task CleanDcpReport(Guid id)
        {
            var report = await _dcpReportsRepo
              .Where(x => x.Id == id)
              .Include(x => x.DcpClassReports)
              .ThenInclude(x => x.Faults)
              .ThenInclude(x => x.RelatedStudents)
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
    }
}
