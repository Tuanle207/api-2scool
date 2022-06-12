using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Common;
using Scool.Dtos;
using Scool.Email;
using Scool.EntityFrameworkCore;
using Scool.IApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Extensions;
using Scool.Permission;
using Scool.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class StatisticsAppService : ScoolAppService, IStatisticsAppService
    {
        private static readonly string EXCEL_MINETYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IRepository<DcpReport, Guid> _dcpReportsRepo;
        private readonly IRepository<DcpClassReport, Guid> _dcpClassReportsRepo;
        private readonly IRepository<DcpClassReportItem, Guid> _dcpReportItemsRepo;
        private readonly IRepository<DcpStudentReport, Guid> _dcpStudentsRepo;
        private readonly IRepository<Student, Guid> _studentsRepo;
        private readonly IRepository<Class, Guid> _classesRepo;
        private readonly IRepository<Teacher, Guid> _teachersRepo;
        private readonly IRepository<Regulation, Guid> _regulationsRepo;
        private readonly ScoolDbContext _context;
        private readonly IEmailSender _emailSender;

        public StatisticsAppService(IRepository<DcpReport, Guid> dcpReportsRepo,
            IRepository<DcpClassReport, Guid> dcpClassReportsRepo,
            IRepository<DcpClassReportItem, Guid> dcpReportItemsRepo,
            IRepository<DcpStudentReport, Guid> dcpStudentsRepo,
            ScoolDbContext context,
            IRepository<Student, Guid> studentsRepo,
            IRepository<Class, Guid> classesRepo,
            IRepository<Regulation, Guid> regulationsRepo,
            IRepository<Teacher, Guid> teachersRepo,
            IEmailSender emailSender)
        {
            _dcpReportsRepo = dcpReportsRepo;
            _dcpClassReportsRepo = dcpClassReportsRepo;
            _dcpReportItemsRepo = dcpReportItemsRepo;
            _dcpStudentsRepo = dcpStudentsRepo;
            _context = context;
            _studentsRepo = studentsRepo;
            _classesRepo = classesRepo;
            _regulationsRepo = regulationsRepo;
            _teachersRepo = teachersRepo;
            _emailSender = emailSender;
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<DcpClassFault>> GetClassesFaults(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter);

            var query = _context.DcpClassFaults.FromSqlRaw(@$"
                SET ARITHABORT ON;
                SELECT 
                    F.Id ClassId,
                    COALESCE(X.Faults, 0) Faults,
                    COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
                    F.Name ClassName,
                    J.Name FormTeacherName 
                FROM  (
                SELECT A.Id ClassId,
                COUNT(D.Id) Faults,
                COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints
                FROM [AppClass] A
                JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
                JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
                JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
                WHERE
                    {FilterActiveCourseSql("A.CourseId")} AND
                    {FilterCurrentTenantSql("A.TenantId ")} AND 
                    (
                        C.Status = N'{DcpReportStatus.Approved}' AND 
                        (
                            DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND 
                            DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0
                        )
                    )
                GROUP BY A.Id
                ) X JOIN [AppClass] F ON F.ID = X.ClassId
                JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId
                ORDER BY Faults DESC, PenaltyPoints DESC, ClassName ASC;
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<DcpClassFault>(items, items.Count);
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<CommonDcpFault>> GetCommonFaults(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter, false);

            var query = _context.CommonDcpFaults.FromSqlRaw(@$"
                SET ARITHABORT ON;
                SELECT 
                Y.Id, 
                Y.DisplayName Name, 
                Z.DisplayName CriteriaName, 
                X.Faults             
                FROM (
                SELECT A.Id, COUNT(B.Id) Faults FROM [AppRegulation] A
                JOIN [AppDcpClassReportItem] B ON A.Id = B.RegulationId
                JOIN [AppDcpClassReport] C ON B.DcpClassReportId = C.Id
                JOIN [AppDcpReport] D ON C.DcpReportId = D.Id
                WHERE
                    {FilterActiveCourseSql("A.CourseId")} AND
                    {FilterCurrentTenantSql("A.TenantId ")} AND 
                    (
                        D.Status = N'{DcpReportStatus.Approved}' AND 
                        (
                            DATEDIFF(DAY, '{input.StartTime}', D.CreationTime) >= 0 AND 
                            DATEDIFF(DAY, D.CreationTime, '{input.EndTime}') >= 0
                        )
                    )
                GROUP BY A.Id
                ) X JOIN [AppRegulation] Y ON X.Id = Y.Id
                JOIN [AppCriteria] Z ON Y.CriteriaId = Z.Id
                WHERE Faults > 0
                ORDER BY X.Faults DESC, Z.Name ASC;
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<CommonDcpFault>(items, items.Count);
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<PagingModel<OverallClassRanking>> GetOverallRanking(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter);

            const int LRRatio = 2;
            const int DCPRatio = 1;
            const int TotalRatio = LRRatio + DCPRatio;

            var queryString = @$"
                WITH LR AS
                (
	                SELECT 
                        CL.Id ClassId, 
                        COALESCE(X.LrPoints, 0) LrPoints,
                        COALESCE(X.TotalAbsence, 0) TotalAbsence
                    FROM (
	                    SELECT 
	                    A.Id ClassId,
	                    SUM(B.TotalPoint) LrPoints,
	                    SUM(B.AbsenceNo) TotalAbsence
	                    FROM [AppClass] A
	                    JOIN [AppLessonsRegister] B ON A.Id = B.ClassId
	                    WHERE
		                    {FilterActiveCourseSql("A.CourseId")} AND
                            {FilterCurrentTenantSql("A.TenantId ")} AND 
		                    (
			                    B.Status = N'Approved' AND 
			                    (
				                    DATEDIFF(DAY, '{input.StartTime}', B.CreationTime) >= 0 AND 
				                    DATEDIFF(DAY, B.CreationTime, '{input.EndTime}') >= 0
			                    )
		                    )
	                    GROUP BY A.Id
                    ) X RIGHT JOIN [AppClass] CL ON CL.Id = X.ClassId
                    WHERE {FilterActiveCourseSql("CL.CourseId")} AND {FilterCurrentTenantSql("CL.TenantId ")}
                )
                SELECT RANK() OVER (
	                ORDER BY R.RankingPoints DESC, 
	                R.LrPoints DESC, 
	                R.TotalAbsence ASC, 
	                R.Faults ASC) Ranking,
	                R.*
                FROM (
	                SELECT
	                CL.Id ClassId,
	                CL.Name ClassName,
	                TC.Name FormTeacherName,
	                LR.TotalAbsence,
	                DCP.Faults,
	                DCP.PenaltyPoints,
	                LR.LrPoints,
	                DCP.DcpPoints,
	                CONVERT(int, ROUND((LR.LrPoints * {LRRatio} + DCP.DcpPoints * {DCPRatio}) / {TotalRatio}, 0)) RankingPoints
	                FROM
	                (
	                    SELECT 
		                    F.Id ClassId,
		                    COALESCE(X.Faults, 0) Faults,
		                    COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
		                    COALESCE(X.TotalPoints, {input.StartPoints}) DcpPoints
	                    FROM  (
		                    SELECT A.Id ClassId,
		                    COUNT(D.Id) Faults,
		                    COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
		                    {input.StartPoints} - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
		                    FROM [AppClass] A
		                    JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
		                    JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
		                    JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
		                    WHERE
                                {FilterActiveCourseSql("A.CourseId")} AND
                                {FilterCurrentTenantSql("A.TenantId ")} AND 
                                (
                                    C.Status = N'{DcpReportStatus.Approved}' AND 
                                    ( 
                                        DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND 
                                        DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0
                                    )
                                )
		                    GROUP BY A.ID
	                    ) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId                                                 -- Re-right join instead of left join above: 
                        WHERE {FilterActiveCourseSql("F.CourseId")} AND {FilterCurrentTenantSql("F.TenantId ")}         -- rejoin 
	                ) DCP 
	                JOIN LR ON DCP.ClassId = LR.ClassId 
	                JOIN [AppClass] CL ON CL.Id = DCP.ClassId 
	                LEFT JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId                                              -- Teacher can be null
                ) R;
                ";

            var query = _context.OverallClassRanking.FromSqlRaw(queryString)
                .AsNoTracking();

            var items = await query.ToListAsync();

            return new PagingModel<OverallClassRanking>(items, items.Count);
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<PagingModel<DcpClassRanking>> GetDcpRanking(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter);

            var query = _context.DcpClassRankings.FromSqlRaw(@$"
                SET ARITHABORT ON;
                SELECT RANK() OVER (ORDER BY R.DcpPoints DESC, R.Faults ASC) Ranking, R.* 
                FROM (
	                SELECT
	                    CL.Id ClassId,
	                    CL.Name ClassName,
	                    TC.Name FormTeacherName,
	                    DCP.Faults,
	                    DCP.PenaltyPoints,
	                    DCP.DcpPoints
	                FROM
	                (
	                    SELECT 
		                    F.Id ClassId,
		                    COALESCE(X.Faults, 0) Faults,
		                    COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
		                    COALESCE(X.TotalPoints, {input.StartPoints}) DcpPoints
	                    FROM  (
		                    SELECT A.Id ClassId,
		                    COUNT(D.Id) Faults,
		                    COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
		                    {input.StartPoints} - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
		                    FROM [AppClass] A
		                    JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
		                    JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
		                    JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
		                    WHERE
                                {FilterActiveCourseSql("A.CourseId")} AND
                                {FilterCurrentTenantSql("A.TenantId ")} AND 
                                (
                                    C.Status = N'{DcpReportStatus.Approved}' AND 
                                    ( 
                                        DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND 
                                        DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0
                                    )
                                )
		                    GROUP BY A.ID
	                    ) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId                                                 -- Re-right join instead of left join above: 
                        WHERE {FilterActiveCourseSql("F.CourseId")} AND {FilterCurrentTenantSql("F.TenantId ")}         -- rejoin 
	                ) DCP 
	                JOIN [AppClass] CL ON CL.Id = DCP.ClassId 
	                LEFT JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId
                ) R
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<DcpClassRanking>(items, items.Count);
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<PagingModel<LrClassRanking>> GetLrRanking(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter);

            var query = _context.LrClassRankings.FromSqlRaw(@$"
               SET ARITHABORT ON;
                SELECT RANK() OVER (ORDER BY R.LrPoints DESC, R.TotalAbsence ASC) Ranking, R.* 
                FROM (
	                SELECT
	                    CL.Id ClassId,
	                    CL.Name ClassName,
	                    TC.Name FormTeacherName,
	                    LR.TotalAbsence,
	                    LR.LrPoints
	                FROM
	                (
	                    SELECT 
                        CL.Id ClassId, 
                        COALESCE(X.LrPoints, 0) LrPoints,
                        COALESCE(X.TotalAbsence, 0) TotalAbsence
                        FROM (
	                        SELECT 
	                        A.Id ClassId,
	                        SUM(B.TotalPoint) LrPoints,
	                        SUM(B.AbsenceNo) TotalAbsence
	                        FROM [AppClass] A
	                        JOIN [AppLessonsRegister] B ON A.Id = B.ClassId
	                        WHERE
		                        {FilterActiveCourseSql("A.CourseId")} AND
                                {FilterCurrentTenantSql("A.TenantId ")} AND 
		                        (
			                        B.Status = N'Approved' AND 
			                        (
				                        DATEDIFF(DAY, '{input.StartTime}', B.CreationTime) >= 0 AND 
				                        DATEDIFF(DAY, B.CreationTime, '{input.EndTime}') >= 0
			                        )
		                        )
	                        GROUP BY A.Id
                        ) X RIGHT JOIN [AppClass] CL ON CL.Id = X.ClassId
                        WHERE {FilterActiveCourseSql("CL.CourseId")} AND {FilterCurrentTenantSql("CL.TenantId ")}
	                ) LR 
	                JOIN [AppClass] CL ON CL.Id = LR.ClassId 
	                LEFT JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId
                ) R
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<LrClassRanking>(items, items.Count);
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<StudentWithMostFaults>> GetStudentsWithMostFaults(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter, false);

            var query = _context.StudentWithMostFaults.FromSqlRaw(@$"
                SET ARITHABORT ON;
                SELECT
                Y.Id Id,
                COALESCE(X.Faults, 0) Faults,
                Y.Name StudentName, 
                Z.Name ClassName FROM
                (
                SELECT A.ID, COUNT(C.Id) Faults 
                FROM [AppStudent] A
                JOIN [AppDcpStudentReport] B ON A.Id = B.StudentId
                JOIN [AppDcpClassReportItem] C ON B.DcpClassReportItemId = C.Id
                JOIN [AppDcpClassReport] D ON C.DcpClassReportId = D.Id
                JOIN [AppDcpReport] E ON D.DcpReportId = E.Id
                WHERE
                    {FilterActiveCourseSql("A.CourseId")} AND
                    {FilterCurrentTenantSql("A.TenantId ")} AND 
                    (
                        E.Status = N'{DcpReportStatus.Approved}' AND 
                        (
                            DATEDIFF(DAY, '{input.StartTime}', E.CreationTime) >= 0 AND 
                            DATEDIFF(DAY, E.CreationTime, '{input.EndTime}') >= 0
                        )
                    )
                GROUP BY A.Id
                ) X JOIN [AppStudent] Y ON X.Id = Y.Id
                JOIN [AppClass] Z ON Y.ClassId = Z.Id
                WHERE Faults != 0
                ORDER BY Faults DESC, StudentName ASC
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<StudentWithMostFaults>(items, items.Count);
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetClassesFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetClassesFaults(timeFilter);
            var template = GenerateTemplate(new List<DcpClassFault>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetCommonFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetCommonFaults(timeFilter);
            var template = GenerateTemplate(new List<CommonDcpFault>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<IRemoteStreamContent> GetOverallRankingExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetOverallRanking(timeFilter);
            var template = GenerateTemplate(new List<OverallClassRanking>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<IRemoteStreamContent> GetDcpRankingExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetDcpRanking(timeFilter);
            var template = GenerateTemplate(new List<DcpClassRanking>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<IRemoteStreamContent> GetLrRankingExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetLrRanking(timeFilter);
            var template = GenerateTemplate(new List<LrClassRanking>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetStudentsWithMostFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetStudentsWithMostFaults(timeFilter);
            var template = GenerateTemplate(new List<StudentWithMostFaults>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        public async Task<LineChartStatDto> GetStatForLineChart(TimeFilterDto timeFilter)
        {
            var result = new LineChartStatDto
            {
                Items = new Dictionary<string, List<LineChartStat>>()
            };

            var rankings = await GetDcpRanking(timeFilter);

            var top5 = rankings.Items
                .OrderByDescending(x => x.DcpPoints)
                    .ThenBy(x => x.Faults)
                .Take(5).ToList();

            var classIds = top5.Select(x => x.ClassId).ToList();

            var reports = await _dcpReportsRepo.AsNoTracking()
                .Include(x => x.DcpClassReports)
                    .ThenInclude(x => x.Faults)
                .Where(x => 
                    x.Status == DcpReportStatus.Approved && 
                    x.CreationTime >= timeFilter.StartTime && x.CreationTime <= timeFilter.EndTime && 
                    x.DcpClassReports.Any(c => classIds.Contains(c.ClassId))
                )
                .ToListAsync();

            var dayofWeeks = new Dictionary<int, string>
            {
                { 0, "T2" },
                { 1, "T3" },
                { 2, "T4" },
                { 3, "T5" },
                { 4, "T6" },
                { 5, "T7" },
                { 6, "CN" },
            };

            for (int dayofWeek = 0; dayofWeek <= 6; dayofWeek++) // 0 = monday, 1 = tuesday, ...
            {
                var stats = new List<LineChartStat>();
                DateTime currentDate = timeFilter.StartTime.Date.AddDays(dayofWeek);
                DateTime nextDate = currentDate.AddDays(1);
                var classReports = reports
                    .Where(x => x.CreationTime >= currentDate && x.CreationTime < nextDate)
                    .SelectMany(x => x.DcpClassReports)
                    .ToList();

                foreach (Guid classId in classIds)
                {
                    var currentClassReports = classReports.Where(x => x.ClassId == classId);
                    int penaltyPoints = currentClassReports.Sum(x => x.PenaltyTotal);
                    int faults = currentClassReports.SelectMany(x => x.Faults).Count();
                    stats.Add(new LineChartStat
                    {
                        ClassId = classId,
                        Faults = faults,
                        PenaltyPoint = penaltyPoints
                    });
                }

                result.Items.Add(dayofWeeks[dayofWeek], stats);
            }

            return result;
        }

        public async Task<PieChartStatDto> GetStatForPieChart(TimeFilterDto timeFilter)
        {
            var stats = await GetCommonFaults(timeFilter);

            var top5 = stats.Items
                .Take(5)
                .OrderByDescending(x => x.Faults)
                    .ThenBy(x => x.Name)
                .ToList();

            var others = stats.Items.Skip(5).ToList();

            var otherFaults = new CommonDcpFault
            {
                Name = "Vi phạm khác",
                Faults = others.Sum(x => x.Faults)
            };

            if (others.Count > 0 && otherFaults.Faults > 0)
            {
                top5.Add(otherFaults);
            }

            return new PieChartStatDto
            {
                Items = top5.Select(x => new PieChartStat
                {
                    Name = x.Name,
                    Value = x.Faults
                }).ToList()
            };
        }

        public async Task<BarChartStatDto> GetStatForBarChart(TimeFilterDto timeFilter)
        {
            var stats = await GetDcpRanking(timeFilter);

            var top5 = stats.Items
                .Take(5)
                .OrderByDescending(x => x.DcpPoints)
                    .ThenBy(x => x.Faults)
                .ToList();

            return new BarChartStatDto
            {
                Items = top5.Select(x => new BarChartStat
                {
                    Name = x.ClassName,
                    Points = x.DcpPoints,
                    Faults = x.Faults
                }).ToList()
            };
        }


        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<ClassFaultDetail>> GetClassFaultDetails([FromRoute(Name = "classId")] Guid classId, TimeFilterDto filter)
        {
            var (startTime, endTime) = ParseDateTime(filter);

            var query = _dcpClassReportsRepo.AsNoTracking()
                .Include(x => x.DcpReport)
                .Where(x => x.ClassId == classId && x.DcpReport.Status == DcpReportStatus.Approved && x.DcpReport.CreationTime.Date >= startTime.Date && x.DcpReport.CreationTime.Date <= endTime.Date)
                .ToQueryString();

            var classReportIdTimes = await _dcpClassReportsRepo.AsNoTracking()
                .Include(x => x.DcpReport)
                .Where(x => x.ClassId == classId && x.DcpReport.Status == DcpReportStatus.Approved && x.DcpReport.CreationTime.Date >= startTime.Date && x.DcpReport.CreationTime.Date <= endTime.Date)
                .Select(x => new { x.Id, x.DcpReport.CreationTime })
                .ToDictionaryAsync(x => x.Id, x => x.CreationTime);

            var classReportIds = classReportIdTimes.Keys.ToList();

            var reportItems = await _dcpReportItemsRepo.AsNoTracking()
                .Include(x => x.RelatedStudents)
                .Where(x => classReportIds.Contains(x.DcpClassReportId))
                .ToListAsync();

            var uniqueRegulationIds = reportItems.Select(x => x.RegulationId).ToList();
            var uniqueStudentIds = reportItems.SelectMany(x => x.RelatedStudents).Select(x => x.StudentId).ToList();

            var regulations = await _regulationsRepo.AsNoTracking()
                .Include(x => x.Criteria)
                .Where(x => uniqueRegulationIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Regulation, RegulationForSimpleListDto>(x))
                .ToListAsync();

            var students = await _studentsRepo.AsNoTracking()
                .Where(x => uniqueStudentIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Student, StudentForSimpleListDto>(x))
                .ToListAsync();

            var items = new List<ClassFaultDetail>();

            foreach (var reportItem in reportItems)
            {
                var regulation = regulations.FirstOrDefault(x => x.Id == reportItem.RegulationId);
                var reportIdTime = classReportIdTimes.FirstOrDefault(x => x.Key == reportItem.DcpClassReportId);

                var reportStudentIds = reportItem.RelatedStudents.Select(x => x.StudentId).ToList();
                var reportstudents = students.Where(x => reportStudentIds.Contains(x.Id)).ToList();

                items.Add(new ClassFaultDetail
                {
                    Id = reportItem.Id,
                    RegulationId = reportItem.RegulationId,
                    RegulationName = regulation.Name,
                    CriteriaName = regulation.Criteria.Name,
                    CreationTime = reportIdTime.Value,
                    PenaltyPoints = reportItem.PenaltyPoint,
                    StudentNames = reportstudents.Select(x => x.Name).JoinAsString(", "),
                });
            }

            return new PagingModel<ClassFaultDetail>(items
                .OrderBy(x => x.CreationTime)
                .ThenBy(x => x.RegulationName)
                );
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<FaultDetail>> GetRegulationFaultDetails([FromRoute(Name = "regulationId")] Guid regulationId, TimeFilterDto filter)
        {
            var (startTime, endTime) = ParseDateTime(filter, false);

            var reportItemIdTimes = await _dcpReportItemsRepo.AsNoTracking()
                .Include(x => x.DcpClassReport)
                    .ThenInclude(x => x.DcpReport)
                .Where(x => x.RegulationId == regulationId && x.DcpClassReport.DcpReport.Status == DcpReportStatus.Approved && x.DcpClassReport.DcpReport.CreationTime.Date >= startTime && x.DcpClassReport.DcpReport.CreationTime.Date <= endTime)
                .Select(x => new { x.Id, x.DcpClassReport.DcpReport.CreationTime })
                .ToDictionaryAsync(x => x.Id, x => x.CreationTime);

            var reportItemIds = reportItemIdTimes.Keys.ToList();

            var reportItems = await _dcpReportItemsRepo.AsNoTracking()
                .Include(x => x.RelatedStudents)
                .Include(x => x.DcpClassReport)
                .Where(x => reportItemIds.Contains(x.Id))
                .ToListAsync();

            var uniqueStudentIds = reportItems.SelectMany(x => x.RelatedStudents).Select(x => x.StudentId).ToList();
            var uniqueClassIds = reportItems.Select(x => x.DcpClassReport.ClassId).ToList();

            var students = await _studentsRepo.AsNoTracking()
                .Include(x => x.Class)
                .Where(x => uniqueStudentIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Student, StudentDto>(x))
                .ToListAsync();

            var classes = await _classesRepo.AsNoTracking()
                .Where(x => uniqueClassIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Class, ClassForSimpleListDto>(x))
                .ToListAsync();

            var items = new List<FaultDetail>();

            foreach (var reportItem in reportItems)
            {
                var reportIdTime = reportItemIdTimes.FirstOrDefault(x => x.Key == reportItem.Id);

                var reportStudentIds = reportItem.RelatedStudents.Select(x => x.StudentId).ToList();
                var reportstudents = students.Where(x => reportStudentIds.Contains(x.Id)).ToList();
                var className = classes.FirstOrDefault(x => x.Id == reportItem.DcpClassReport.ClassId).Name;

                items.Add(new FaultDetail
                {
                    Id = reportItem.Id,
                    CreationTime = reportIdTime.Value,
                    PenaltyPoints = reportItem.PenaltyPoint,
                    StudentNames = reportstudents.Select(x => x.Name).JoinAsString(", "),
                    ClassName = className
                });
            }

            return new PagingModel<FaultDetail>(items
                .OrderBy(x => x.CreationTime)
                .ThenBy(x => x.PenaltyPoints)
                );
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<StudentFaultDetail>> GetStudentFaultDetails([FromRoute(Name = "studentId")] Guid studentId, TimeFilterDto filter)
        {
            var (startTime, endTime) = ParseDateTime(filter, false);

            var reportItemIdTimes = await _dcpStudentsRepo.AsNoTracking()
                .Include(x => x.DcpClassReportItem)
                    .ThenInclude(x => x.DcpClassReport)
                        .ThenInclude(x => x.DcpReport)
                .Where(x => x.StudentId == studentId && x.DcpClassReportItem.DcpClassReport.DcpReport.Status == DcpReportStatus.Approved && x.DcpClassReportItem.DcpClassReport.DcpReport.CreationTime.Date >= startTime && x.DcpClassReportItem.DcpClassReport.DcpReport.CreationTime.Date <= endTime)
                .Select(x => new { x.DcpClassReportItem.Id, x.DcpClassReportItem.DcpClassReport.DcpReport.CreationTime })
                .ToDictionaryAsync(x => x.Id, x => x.CreationTime);

            var reportItemIds = reportItemIdTimes.Keys.ToList();

            var reportItems = await _dcpReportItemsRepo.AsNoTracking()
                .Include(x => x.RelatedStudents)
                .Where(x => reportItemIds.Contains(x.Id))
                .ToListAsync();

            var uniqueRegulationIds = reportItems.Select(x => x.RegulationId).ToList();
            var regulations = await _regulationsRepo.AsNoTracking()
                .Include(x => x.Criteria)
                .Where(x => uniqueRegulationIds.Contains(x.Id))
                .Select(x => ObjectMapper.Map<Regulation, RegulationForSimpleListDto>(x))
                .ToListAsync();

            var items = new List<StudentFaultDetail>();

            foreach (var reportItem in reportItems)
            {
                var reportIdTime = reportItemIdTimes.FirstOrDefault(x => x.Key == reportItem.Id);
                var regulation = regulations.FirstOrDefault(x => x.Id == reportItem.RegulationId);

                items.Add(new StudentFaultDetail
                {
                    Id = reportItem.Id,
                    CreationTime = reportIdTime.Value,
                    PenaltyPoints = reportItem.PenaltyPoint,
                    RegulationName = regulation.Name,
                    CriteriaName = regulation.Name,
                    Count = 1
                });
            }

            return new PagingModel<StudentFaultDetail>(items
                .OrderBy(x => x.CreationTime)
                .ThenBy(x => x.PenaltyPoints)
                );
        }

        //[Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetClassFaultDetailsExcel([FromRoute(Name = "classId")] Guid classId, TimeFilterDto timeFilter)
        {
            var stats = await GetClassFaultDetails(classId, timeFilter);
            var template = GenerateTemplate(new List<ClassFaultDetail>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        //[Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetRegulationFaultDetailsExcel([FromRoute(Name = "regulationId")] Guid regulationId, TimeFilterDto timeFilter)
        {
            var stats = await GetRegulationFaultDetails(regulationId, timeFilter);
            var template = GenerateTemplate(new List<FaultDetail>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        //[Authorize(StatsPermissions.Statistics)]
        public async Task<IRemoteStreamContent> GetStudentFaultDetailsExcel([FromRoute(Name = "studentId")]Guid studentId, TimeFilterDto timeFilter)
        {
            var stats = await GetStudentFaultDetails(studentId, timeFilter);
            var template = GenerateTemplate(new List<StudentFaultDetail>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);

            return new RemoteStreamContent(outputStream)
            {
                ContentType = EXCEL_MINETYPE
            };
        }

        [Authorize(StatsPermissions.Statistics)]
        [HttpPost("api/app/statistics/send-class-faults-through-email")]
        public async Task SendClassFaultsThroughEmail([FromQuery(Name = "classId")]Guid? classId, [FromQuery(Name = "startTime")] DateTime inputStartTime, [FromQuery(Name = "endTime")] DateTime inputEndTime)
        {
            var input = new TimeFilterDto
            {
                StartTime = inputStartTime,
                EndTime = inputEndTime
            };
            var (startTime, endTime) = ParseDateTime(input);
            var timeRange = $"{startTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}-{endTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            if (classId.HasValue)
            {
                var classInfo = await _classesRepo.AsNoTracking()
                    .Include(x => x.FormTeacher)
                    .FirstOrDefaultAsync(x => x.Id == classId.Value);

                if (classInfo == null || string.IsNullOrEmpty(classInfo.FormTeacher?.Email))
                {
                    return;
                }

                var emailArgs = new ReportEmailSendingArgs
                {
                    To = new List<string> { classInfo.FormTeacher.Email },
                    Subject = $"2Scool - Báo cáo thống kê lỗi vi phạm {classInfo.Name} từ ngày {timeRange}",
                    Body = $"Báo cáo thống kê lỗi vi phạm {classInfo.Name} từ ngày {timeRange}",
                    ReportType = EmailReportType.Class,
                    ReportName = $"bao-cao-loi-vi-pham-lop-{classInfo.Name}-{timeRange}.xlsx",
                    EntityId = classInfo.Id,
                    StartTime = startTime,
                    EndTime = endTime,
                };

                await _emailSender.QueueAsync(emailArgs);

                return;
            }

            var classInfos = await _classesRepo.AsNoTracking()
                .Include(x => x.FormTeacher)
                .Where(x => x.CourseId == ActiveCourse.Id.Value && x.FormTeacherId.HasValue && !string.IsNullOrEmpty(x.FormTeacher.Email))
                .ToListAsync();

            var emailArgsList = classInfos.Select(classInfo => new ReportEmailSendingArgs
            {
                To = new List<string> { classInfo.FormTeacher.Email },
                Subject = $"2Scool - Báo cáo thống kê lỗi vi phạm {classInfo.Name} từ ngày {timeRange}",
                Body = $"Báo cáo thống kê lỗi vi phạm {classInfo.Name} từ ngày {timeRange}",
                ReportType = EmailReportType.Class,
                ReportName = $"bao-cao-loi-vi-pham-lop-{classInfo.Name}-{timeRange}.xlsx",
                EntityId = classInfo.Id,
                StartTime = startTime,
                EndTime = endTime,
            });

            var sendEmailTasks = emailArgsList.Select(x => _emailSender.QueueAsync(x));
            await Task.WhenAll(sendEmailTasks);
        }

        [Authorize(StatsPermissions.Statistics)]
        [HttpPost("api/app/statistics/send-student-faults-through-email")]
        public async Task SendStudentFaultsThroughEmail([FromQuery(Name = "studentId")] Guid studentId, [FromQuery(Name = "startTime")] DateTime inputStartTime, [FromQuery(Name = "endTime")] DateTime inputEndTime)
        {
            var input = new TimeFilterDto
            {
                StartTime = inputStartTime,
                EndTime = inputEndTime
            };
            var (startTime, endTime) = ParseDateTime(input, false);
            var timeRange = $"{startTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}-{endTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}";

            var studentInfo = await _studentsRepo.AsNoTracking()
                    .Include(x => x.Class)
                        .ThenInclude(x => x.FormTeacher)
                    .FirstOrDefaultAsync(x => x.Id == studentId);

            if (studentInfo == null || string.IsNullOrEmpty(studentInfo.Class.FormTeacher?.Email))
            {
                return;
            }

            var emailArgs = new ReportEmailSendingArgs
            {
                To = new List<string> { studentInfo.Class.FormTeacher.Email },
                Subject = $"2Scool - Báo cáo lỗi vi phạm của học sinh {studentInfo.Name} từ ngày {timeRange}",
                Body = $"Báo cáo thống kê lỗi vi phạm {studentInfo.Name} từ ngày {timeRange}",
                ReportType = EmailReportType.Student,
                ReportName = $"bao-cao-loi-vi-pham-hoc-sinh-{studentInfo.Name}-{timeRange}.xlsx",
                EntityId = studentId,
                StartTime = startTime,
                EndTime = endTime,
            };

            await _emailSender.QueueAsync(emailArgs);
        }

        private string FilterCurrentTenantSql(string expression)
        {
            if (CurrentTenant.Id.HasValue)
            {
                return $"{expression} = '{CurrentTenant.Id.Value}'";
            }
            return $"{expression} IS NULL";
        }

        private string FilterActiveCourseSql(string expression)
        {
            if (ActiveCourse.Id.HasValue)
            {
                return $"{expression} = '{ActiveCourse.Id.Value}'";
            }
            return $"{expression} IS NULL";
        }

        private StatisticsQueryInput ParseQueryInput(TimeFilterDto timeFilter, bool byWeek = true)
        {
            var startDate = byWeek && timeFilter.StartTime.DayOfWeek != DayOfWeek.Monday ?
                timeFilter.StartTime.AddDays(7).StartOfWeek() : timeFilter.StartTime;
            var durations = timeFilter.EndTime - startDate;
            var weeks = (int)Math.Ceiling(durations.TotalDays / 7);
            if (weeks == 0)
            {
                weeks = 1;
            }

            return new StatisticsQueryInput
            {
                StartPoints = weeks * 100,
                StartTime = startDate.ToString("MM/dd/yyyy"),
                EndTime = timeFilter.EndTime.ToString("MM/dd/yyyy"),
                Weeks = weeks
            };
        }

        private (DateTime, DateTime) ParseDateTime(TimeFilterDto timeFilter, bool byWeek = true)
        {
            var startDate = byWeek && timeFilter.StartTime.DayOfWeek != DayOfWeek.Monday ?
                timeFilter.StartTime.AddDays(7).StartOfWeek() : timeFilter.StartTime;

            return (startDate.Date, timeFilter.EndTime.Date);
        }

        private static XLWorkbook GenerateTemplate(List<DcpClassFault> stats, TimeFilterDto timeFilter)
        {

            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê lớp vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;

            ws.Cell(3, 1).SetValue("Mã lớp");
            ws.Cell(3, 2).SetValue("Tên lớp");
            ws.Cell(3, 3).SetValue("Giáo viên chủ nhiệm");
            ws.Cell(3, 4).SetValue("Lượt vi phạm");
            ws.Cell(3, 5).SetValue("Tổng điểm trừ");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.ClassId);
                ws.Cell(index, 2).SetValue(stat.ClassName);
                ws.Cell(index, 3).SetValue(stat.FormTeacherName);
                ws.Cell(index, 4).SetValue(stat.Faults);
                ws.Cell(index, 5).SetValue(stat.PenaltyPoints);
                index += 1;
            }
            
            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<OverallClassRanking> stats, TimeFilterDto timeFilter)
        {

            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Báo cáo xếp hạng thi đua");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 20;
            ws.Column(7).Width = 20;
            ws.Column(8).Width = 20;
            ws.Column(9).Width = 20;
            ws.Column(10).Width = 20;

            ws.Cell(3, 1).SetValue("Thứ hạng");
            ws.Cell(3, 2).SetValue("Tên lớp");
            ws.Cell(3, 3).SetValue("Giáo viên chủ nhiệm");
            ws.Cell(3, 4).SetValue("Lượt vắng");
            ws.Cell(3, 5).SetValue("Lượt vi phạm");
            ws.Cell(3, 6).SetValue("Điểm trừ");
            ws.Cell(3, 7).SetValue("Điểm sổ đầu bài");
            ws.Cell(3, 8).SetValue("Điểm nề nếp");
            ws.Cell(3, 9).SetValue("Điểm thi đua");

            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 8).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 9).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;
            ws.Cell(3, 6).Style.Font.Bold = true;
            ws.Cell(3, 7).Style.Font.Bold = true;
            ws.Cell(3, 8).Style.Font.Bold = true;
            ws.Cell(3, 9).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Ranking);
                ws.Cell(index, 2).SetValue(stat.ClassName);
                ws.Cell(index, 3).SetValue(stat.FormTeacherName);
                ws.Cell(index, 4).SetValue(stat.TotalAbsence);
                ws.Cell(index, 5).SetValue(stat.Faults);
                ws.Cell(index, 6).SetValue(stat.PenaltyPoints);
                ws.Cell(index, 7).SetValue(stat.LrPoints);
                ws.Cell(index, 8).SetValue(stat.DcpPoints);
                ws.Cell(index, 9).SetValue(stat.RankingPoints);

                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<DcpClassRanking> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Báo cáo xếp hạng thi đua nề nếp");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 20;

            ws.Cell(3, 1).SetValue("Thứ hạng");
            ws.Cell(3, 2).SetValue("Tên lớp");
            ws.Cell(3, 3).SetValue("Giáo viên chủ nhiệm");
            ws.Cell(3, 4).SetValue("Lượt vi phạm");
            ws.Cell(3, 5).SetValue("Điểm trừ");
            ws.Cell(3, 6).SetValue("Tổng điểm");

            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;
            ws.Cell(3, 6).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Ranking);
                ws.Cell(index, 2).SetValue(stat.ClassName);
                ws.Cell(index, 3).SetValue(stat.FormTeacherName);
                ws.Cell(index, 4).SetValue(stat.Faults);
                ws.Cell(index, 5).SetValue(stat.PenaltyPoints);
                ws.Cell(index, 6).SetValue(stat.DcpPoints);

                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<LrClassRanking> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Báo cáo xếp hạng sổ đầu bài");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;

            ws.Cell(3, 1).SetValue("Thứ hạng");
            ws.Cell(3, 2).SetValue("Tên lớp");
            ws.Cell(3, 3).SetValue("Giáo viên chủ nhiệm");
            ws.Cell(3, 4).SetValue("Số lượt vắng");
            ws.Cell(3, 5).SetValue("Điểm sổ đầu bài");

            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Ranking);
                ws.Cell(index, 2).SetValue(stat.ClassName);
                ws.Cell(index, 3).SetValue(stat.FormTeacherName);
                ws.Cell(index, 4).SetValue(stat.TotalAbsence);
                ws.Cell(index, 5).SetValue(stat.LrPoints);

                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<CommonDcpFault> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê lỗi vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 100;
            ws.Column(3).Width = 30;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;

            ws.Cell(3, 1).SetValue("Mã quy định");
            ws.Cell(3, 2).SetValue("Tên vi phạm");
            ws.Cell(3, 3).SetValue("Tiêu chí");
            ws.Cell(3, 4).SetValue("Lượt vi phạm");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Id);
                ws.Cell(index, 2).SetValue(stat.Name);
                ws.Cell(index, 3).SetValue(stat.CriteriaName);
                ws.Cell(index, 4).SetValue(stat.Faults);
                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<StudentWithMostFaults> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê học sinh vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 40;
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;

            ws.Cell(3, 1).SetValue("Mã học sinh");
            ws.Cell(3, 2).SetValue("Tên học sinh");
            ws.Cell(3, 3).SetValue("Thuộc lớp");
            ws.Cell(3, 4).SetValue("Số vi phạm");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Id);
                ws.Cell(index, 2).SetValue(stat.StudentName);
                ws.Cell(index, 3).SetValue(stat.ClassName);
                ws.Cell(index, 4).SetValue(stat.Faults);
                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<ClassFaultDetail> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 40;

            ws.Cell(3, 1).SetValue("Mã vi phạm");
            ws.Cell(3, 2).SetValue("Thời gian");
            ws.Cell(3, 3).SetValue("Tên quy định");
            ws.Cell(3, 4).SetValue("Tiêu chí");
            ws.Cell(3, 5).SetValue("Tổng điểm trừ");
            ws.Cell(3, 6).SetValue("Học sinh vi phạm");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;
            ws.Cell(3, 6).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Id);
                ws.Cell(index, 2).SetValue(stat.CreationTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
                ws.Cell(index, 3).SetValue(stat.RegulationName);
                ws.Cell(index, 4).SetValue(stat.CriteriaName);
                ws.Cell(index, 5).SetValue(stat.PenaltyPoints);
                ws.Cell(index, 6).SetValue(stat.StudentNames);
                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<FaultDetail> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 40;

            ws.Cell(3, 1).SetValue("Mã vi phạm");
            ws.Cell(3, 2).SetValue("Thời gian");
            ws.Cell(3, 3).SetValue("Tổng điểm trừ");
            ws.Cell(3, 4).SetValue("Lớp vi phạm");
            ws.Cell(3, 5).SetValue("Học sinh vi phạm");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Id);
                ws.Cell(index, 2).SetValue(stat.CreationTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
                ws.Cell(index, 3).SetValue(stat.PenaltyPoints);
                ws.Cell(index, 4).SetValue(stat.ClassName);
                ws.Cell(index, 5).SetValue(stat.StudentNames);
                index += 1;
            }

            return wb;
        }

        private static XLWorkbook GenerateTemplate(List<StudentFaultDetail> stats, TimeFilterDto timeFilter)
        {
            var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Thống kê vi phạm");
            ws.Cell(1, 1).SetValue("Từ ngày");
            ws.Cell(1, 2).SetValue(timeFilter.StartTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Cell(1, 3).SetValue("Đến ngày");
            ws.Cell(1, 4).SetValue(timeFilter.EndTime.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 20;

            ws.Cell(3, 1).SetValue("Mã vi phạm");
            ws.Cell(3, 2).SetValue("Thời gian");
            ws.Cell(3, 3).SetValue("Quy định");
            ws.Cell(3, 4).SetValue("Tiêu chí");
            ws.Cell(3, 5).SetValue("Điểm trừ");
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 255, 0);
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 2).Style.Font.Bold = true;
            ws.Cell(3, 3).Style.Font.Bold = true;
            ws.Cell(3, 4).Style.Font.Bold = true;
            ws.Cell(3, 5).Style.Font.Bold = true;

            var index = 4;

            foreach (var stat in stats)
            {
                ws.Cell(index, 1).SetValue(stat.Id);
                ws.Cell(index, 2).SetValue(stat.CreationTime?.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture));
                ws.Cell(index, 3).SetValue(stat.RegulationName);
                ws.Cell(index, 4).SetValue(stat.CriteriaName);
                ws.Cell(index, 5).SetValue(stat.PenaltyPoints);
                index += 1;
            }

            return wb;
        }
    }

    internal class StatisticsQueryInput
    {
        public int StartPoints { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Weeks { get; set; }
    }
}
