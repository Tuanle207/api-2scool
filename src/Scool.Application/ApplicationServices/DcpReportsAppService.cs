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
        private readonly IRepository<Class, Guid> _classesRepo;
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
            IRepository<Class, Guid> classesRepo,
            IGuidGenerator guidGenerator,
            INotificationService notificationService) : base(dcpReportsRepo)
        {
            _dcpReportsRepo = dcpReportsRepo;
            _dcpClassReportsRepo = dcpClassReportsRepo;
            _dcpClassReportItemsRepo = dcpClassReportItemsRepo;
            _dcpStudentReportsRepo = dcpStudentReportsRepo;
            _studentsRepo = studentsRepo;
            _regulationsRepo = regulationsRepo;
            _guidGenerator = guidGenerator;
            _classesRepo = classesRepo;
            _notificationService = notificationService;

            DeletePolicyName = ReportsPermissions.RemoveDcpReport;
        }

        [Authorize(ReportsPermissions.CreateNewDcpReport)]
        public async override Task<DcpReportDto> CreateAsync(CreateUpdateDcpReportDto input)
        {
            var report = new DcpReport(_guidGenerator.Create());
            report.TenantId = CurrentTenant.Id;
            report.CourseId = ActiveCourse.Id.Value;

            var uniqueRegulationIds = input.DcpClassReports.SelectMany(x => x.Faults).Distinct().Select(x => x.RegulationId).ToList();
            var regulations = await _regulationsRepo.AsNoTracking().Where(x => uniqueRegulationIds.Contains(x.Id)).ToListAsync();
            var classIds = input.DcpClassReports.Select(x => x.ClassId).ToList();
            var classNames = await _classesRepo.AsNoTracking().Where(x => classIds.Contains(x.Id)).Select(x => x.Name).ToListAsync();
            report.ReportedClassDisplayNames = string.Join(", ", classNames);

            // report on each class
            IList<CreateUpdateDcpClassReportDto> clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                int penaltyTotal = 0;

                dcpClassReport.TenantId = CurrentTenant.Id;
                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reportItem in faults)
                {
                    IList<Guid> listStudentId = reportItem.RelatedStudentIds;
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.TenantId = CurrentTenant.Id;
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reportItem.RegulationId;

                    // accumulate penalty
                    Regulation regulation = regulations.FirstOrDefault(x => x.Id == reportItem.RegulationId);
                    int mutiply = regulation.Type == RegulationType.Student && listStudentId.Count > 0 ? listStudentId.Count : 1;
                    var penalty = regulation.Point * mutiply;
                    penaltyTotal += penalty;
                    dcpClassReportItem.PenaltyPoint = penaltyTotal;

                    // students violating the regulations
                    var reportedStudents = listStudentId.Select(studentId => new DcpStudentReport
                    {
                        DcpClassReportItemId = dcpClassReportItem.Id,
                        StudentId = studentId,
                        TenantId = CurrentTenant.Id
                    }).ToList();
                    dcpClassReportItem.RelatedStudents = reportedStudents;
                    await _dcpStudentReportsRepo.InsertManyAsync(reportedStudents);

                    // add dcpClassReportItem report to dcpClassReport
                    dcpClassReport.Faults.Add(
                        await _dcpClassReportItemsRepo.InsertAsync(dcpClassReportItem)
                    );
                }

                // save penalty point for class
                dcpClassReport.PenaltyTotal = penaltyTotal;

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
            report.CourseId = ActiveCourse.Id.Value;
            report.CreationTime = oldReport.CreationTime;

            var uniqueRegulationIds = input.DcpClassReports.SelectMany(x => x.Faults).Distinct().Select(x => x.RegulationId).ToList();
            var regulations = await _regulationsRepo.AsNoTracking().Where(x => uniqueRegulationIds.Contains(x.Id)).ToListAsync();
            var classIds = input.DcpClassReports.Select(x => x.ClassId).ToList();
            var classNames = await _classesRepo.AsNoTracking().Where(x => classIds.Contains(x.Id)).Select(x => x.Name).ToListAsync();
            report.ReportedClassDisplayNames = string.Join(", ", classNames);

            // report on each class
            IList<CreateUpdateDcpClassReportDto> clsReports = input.DcpClassReports;
            foreach (var cls in clsReports)
            {
                var dcpClassReport = new DcpClassReport(_guidGenerator.Create());
                int penaltyTotal = 0;

                dcpClassReport.TenantId = CurrentTenant.Id;
                dcpClassReport.DcpReportId = report.Id;
                dcpClassReport.ClassId = cls.ClassId;

                var faults = cls.Faults;
                // regulations broken
                foreach (var reportItem in faults)
                {
                    IList<Guid> listStudentId = reportItem.RelatedStudentIds;
                    var dcpClassReportItem = new DcpClassReportItem(_guidGenerator.Create());
                    dcpClassReportItem.TenantId = CurrentTenant.Id;
                    dcpClassReportItem.DcpClassReportId = dcpClassReport.Id;
                    dcpClassReportItem.RegulationId = reportItem.RegulationId;

                    // accumulate penalty
                    Regulation regulation = regulations.FirstOrDefault(x => x.Id == reportItem.RegulationId);
                    int mutiply = regulation.Type == RegulationType.Class ? 1 : listStudentId.Count;
                    var penalty = regulation.Point * mutiply;
                    penaltyTotal += penalty;
                    dcpClassReportItem.PenaltyPoint = penaltyTotal;

                    // students violating the regulations
                    var reportedStudents = listStudentId.Select(studentId => new DcpStudentReport
                    {
                        DcpClassReportItemId = dcpClassReportItem.Id,
                        StudentId = studentId,
                        TenantId = CurrentTenant.Id
                    }).ToList();
                    dcpClassReportItem.RelatedStudents = reportedStudents;
                    await _dcpStudentReportsRepo.InsertManyAsync(reportedStudents);

                    // add dcpClassReportItem report to dcpClassReport
                    dcpClassReport.Faults.Add(
                        await _dcpClassReportItemsRepo.InsertAsync(dcpClassReportItem)
                    );
                }

                // save penalty point for class
                dcpClassReport.PenaltyTotal = penaltyTotal;

                // add dcpClassReportItem report to dcpClassReport
                report.DcpClassReports.Add(
                    await _dcpClassReportsRepo.InsertAsync(dcpClassReport)
                );
            }

            var dcpReport = await _dcpReportsRepo.InsertAsync(report, autoSave: true);

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
            var report = await _dcpReportsRepo.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => ObjectMapper.Map<DcpReport, DcpReportDto>(x))
                .FirstOrDefaultAsync();

            report.DcpClassReports = await _dcpClassReportsRepo.AsNoTracking()
                .Include(x => x.Faults)
                    .ThenInclude(x => x.RelatedStudents)
                .Where(x => x.DcpReportId == id)
                .Select(x => ObjectMapper.Map<DcpClassReport, DcpClassReportDto>(x))
                .ToListAsync();

            var faults = report.DcpClassReports.SelectMany(x => x.Faults);

            var uniqueClassIds = report.DcpClassReports.Select(x => x.ClassId).ToList();
            var uniqueRegulationIds = faults.Select(x => x.RegulationId).Distinct().ToList();
            var uniqueStudentIds = faults.SelectMany(x => x.RelatedStudents).Select(x => x.StudentId).Distinct().ToList();

            var uniqueRegulations = await _regulationsRepo.AsNoTracking()
                .Include(x => x.Criteria)
                .Where(x => uniqueRegulationIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Regulation, RegulationForSimpleListDto>(x))
                .ToListAsync();

            var uniqueStudents = await _studentsRepo.AsNoTracking()
                .Where(x => uniqueStudentIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Student, StudentForSimpleListDto>(x))
                .ToListAsync();

            var uniqueClasses = await _classesRepo.AsNoTracking()
                .Where(x => uniqueClassIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Class, ClassForSimpleListDto>(x))
                .ToListAsync();

            foreach (var classReport in report.DcpClassReports)
            {
                classReport.Class = uniqueClasses.FirstOrDefault(x => x.Id == classReport.ClassId);
                foreach (var fault in classReport.Faults)
                {
                    fault.Regulation = uniqueRegulations.FirstOrDefault(x => x.Id == fault.RegulationId);
                    foreach (var student in fault.RelatedStudents)
                    {
                        student.Student = uniqueStudents.FirstOrDefault(x => x.Id == student.StudentId);
                    }
                }
            }

            return report;
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
            var items = await _dcpClassReportsRepo
               .Include(e => e.Faults)
               .ThenInclude(e => e.RelatedStudents)
               .Where(x => x.DcpReportId == id)
               .Select(x => ObjectMapper.Map<DcpClassReport, CreateUpdateDcpClassReportDto>(x))
               .ToListAsync();

            return new CreateUpdateDcpReportDto
            {
                DcpClassReports = items
            };
        }

        [Authorize(ReportsPermissions.GetMyDcpReport)]
        public async Task<PagingModel<DcpReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;

            // Create query with filters
            var query = _dcpReportsRepo.AsNoTracking().Filter(input.Filter);

            // Filter by current user
            if (!CurrentAccount.HasAccount || !ActiveCourse.IsAvailable)
            {
                return new PagingModel<DcpReportDto>();
            }
            query = query.Where(x => x.CreatorId == CurrentAccount.Id.Value && x.CourseId == ActiveCourse.Id);

            // Count total count
            int totalCount = await query.CountAsync();

            // Sort descending by time
            query = query.OrderByDescending(x => x.CreationTime);

            // Pagination
            query = query.Page(pageIndex, pageSize);

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
            var query = _dcpReportsRepo.AsNoTracking().Filter(input.Filter);

            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<DcpReportDto>();
            }
            query = query.Where(x => x.CourseId == ActiveCourse.Id);

            int totalCount = await query.CountAsync();

            // Sort descending by time
            query = query.OrderByDescending(x => x.CreationTime);

            // Pagination
            query = query.Page(pageIndex, pageSize);

            // Include navigation properties
            query = query.Include(x => x.CreatorAccount);

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
