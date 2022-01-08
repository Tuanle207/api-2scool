using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scool.AppConsts;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Application.Permissions;
using Scool.Domain.Common;
using Scool.Domain.Views;
using Scool.Dtos;
using Scool.EntityFrameworkCore;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class StatisticsAppService : ScoolAppService, IStatisticsAppService
    {
        private readonly IRepository<DcpReport, Guid> _dcpReportsRepo;
        private readonly IRepository<DcpClassReport, Guid> _dcpClassReportsRepo;
        private readonly ScoolDbContext _context;

        public StatisticsAppService(IRepository<DcpReport, Guid> dcpReportsRepo, 
            IRepository<DcpClassReport, Guid> dcpClassReportsRepo, ScoolDbContext context)
        {
            _dcpReportsRepo = dcpReportsRepo;
            _dcpClassReportsRepo = dcpClassReportsRepo;
            _context = context;
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
                LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
                LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
                LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
                LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
                WHERE (C.Status = 'Approved') AND (
                    (DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0)
                )
                GROUP BY A.Id
                ) X RIGHT JOIN [AppClass] F ON F.ID = X.ClassId
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
                LEFT JOIN [AppDcpClassReportItem] B ON A.Id = B.RegulationId
                LEFT JOIN [AppDcpClassReport] C ON B.DcpClassReportId = C.Id
                LEFT JOIN [AppDcpReport] D ON C.DcpReportId = D.Id
                LEFT JOIN [AppClass] E ON C.ClassId = E.Id
                WHERE (D.Status = 'Approved') AND (
                    (DATEDIFF(DAY, '{input.StartTime}', D.CreationTime) >= 0 AND DATEDIFF(DAY, D.CreationTime, '{input.EndTime}') >= 0) 
                    OR B.Id IS NULL 
                )
                GROUP BY A.Id
                ) X RIGHT JOIN [AppRegulation] Y ON X.Id = Y.Id
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
                SET ARITHABORT ON;
                WITH LR AS
                (
	                SELECT 
	                B.Id ClassId, 
	                COALESCE(X.LrPoints, 0) LrPoints,
	                COALESCE(X.TotalAbsence, 0) TotalAbsence
	                FROM (
	                SELECT 
	                A.ClassId ClassId,
	                SUM(A.TotalPoint) LrPoints,
	                SUM(A.AbsenceNo) TotalAbsence
	                FROM [AppLessonsRegister] A
	                WHERE (A.Status = N'{DcpReportStatus.Approved}') AND ((DATEDIFF(DAY, '{input.StartTime}', A.CreationTime) >= 0 
                    AND DATEDIFF(DAY, A.CreationTime, '{input.EndTime}') >= 0))
	                GROUP BY A.ClassId
	                ) X RIGHT JOIN [AppClass] B ON B.Id = X.ClassId
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
		                LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
		                LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
		                LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
		                LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
		                WHERE (C.Status = N'{DcpReportStatus.Approved}') AND ((DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 
                        AND DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0))
		                GROUP BY A.ID
	                ) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId
	                ) DCP 
	                JOIN LR ON DCP.ClassId = LR.ClassId 
	                JOIN [AppClass] CL ON CL.Id = DCP.ClassId 
	                JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId
                ) R;
                SET ARITHABORT OFF;
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
                SELECT R.*, RANK() OVER (ORDER BY R.TotalPoints DESC, R.Faults ASC) Ranking 
                FROM (SELECT 
                F.Id ClassId,
                COALESCE(X.Faults, 0) Faults,
                COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
                COALESCE(X.TotalPoints, {input.StartPoints}) TotalPoints,
                F.Name ClassName, 
                J.Name FormTeacherName 
                FROM  (
                SELECT A.Id ClassId,
                COUNT(D.Id) Faults,
                COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
                {input.StartPoints} - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
                FROM [AppClass] A
                LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
                LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
                LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
                LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
                WHERE (C.Status = 'Approved') AND (
                    (DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0) 
                    OR D.Id IS NULL 
                )
                GROUP BY A.ID
                ) X RIGHT JOIN [AppClass] F ON F.ID = X.ClassId
                JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId) R
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<DcpClassRanking>(items, items.Count);
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
                LEFT JOIN [AppDcpStudentReport] B ON A.Id = B.StudentId
                LEFT JOIN [AppDcpClassReportItem] C ON B.DcpClassReportItemId = C.Id
                LEFT JOIN [AppDcpClassReport] D ON C.DcpClassReportId = D.Id
                LEFT JOIN [AppDcpReport] E ON D.DcpReportId = E.Id
                WHERE (E.Status = 'Approved') AND (
                    (DATEDIFF(DAY, '{input.StartTime}', E.CreationTime) >= 0 AND DATEDIFF(DAY, E.CreationTime, '{input.EndTime}') >= 0) 
                    OR B.Id IS NULL
                )
                GROUP BY A.Id
                ) X RIGHT JOIN [AppStudent] Y ON X.Id = Y.Id
                JOIN [AppClass] Z ON Y.ClassId = Z.Id
                WHERE Faults != 0
                ORDER BY Faults DESC, StudentName ASC
                SET ARITHABORT OFF;
            ");

            var items = await query.ToListAsync();

            return new PagingModel<StudentWithMostFaults>(items, items.Count);
        }

        public async Task<MemoryStream> GetClassesFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetClassesFaults(timeFilter);
            var template = GenerateTemplate(new List<DcpClassFault>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);
            return outputStream;
        }

        public async Task<MemoryStream> GetCommonFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetCommonFaults(timeFilter);
            var template = GenerateTemplate(new List<CommonDcpFault>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);
            return outputStream;
        }

        public async Task<MemoryStream> GetOverallRankingExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetOverallRanking(timeFilter);
            var template = GenerateTemplate(new List<OverallClassRanking>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);
            return outputStream;
        }

        public async Task<MemoryStream> GetDcpRankingExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetDcpRanking(timeFilter);
            var template = GenerateTemplate(new List<DcpClassRanking>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);
            return outputStream;
        }

        public async Task<MemoryStream> GetStudentsWithMostFaultsExcel(TimeFilterDto timeFilter)
        {
            var stats = await GetStudentsWithMostFaults(timeFilter);
            var template = GenerateTemplate(new List<StudentWithMostFaults>(stats.Items), timeFilter);
            var outputStream = new MemoryStream();
            template.SaveAs(outputStream);
            return outputStream;
        }

        public async Task<LineChartStatDto> GetStatForLineChart(TimeFilterDto timeFilter)
        {
            var result = new LineChartStatDto
            {
                items = new Dictionary<string, List<LineChartStat>>()
            };

            IList<DcpReport> reports = await _dcpReportsRepo.AsNoTracking()
                .Where(x => x.Status == DcpReportStatus.Approved)
                .Where(x => x.CreationTime >= timeFilter.StartTime && x.CreationTime < timeFilter.EndTime)
                .Include(x => x.DcpClassReports)
                .ThenInclude(x => x.Faults)
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

            for (int dayofWeek = 0; dayofWeek < 6; dayofWeek++) // 0 = monday, 1 = tuesday, ...
            {
                var stats = new List<LineChartStat>();
                DateTime currentDate = timeFilter.StartTime.Date.AddDays(dayofWeek);
                DateTime nextDate = currentDate.AddDays(1);
                IList<DcpClassReport> classReports = reports
                    .Where(x => x.CreationTime >= currentDate && x.CreationTime < nextDate)
                    .SelectMany(x => x.DcpClassReports)
                    .ToList();

                IList<Guid> listClassId = classReports.Select(x => x.ClassId)
                    .Distinct()
                    .ToList();

                foreach (Guid classId in listClassId)
                {
                    IEnumerable<DcpClassReport> currentClassReports = classReports.Where(x => x.ClassId == classId);

                    int penaltyPoints = currentClassReports.Sum(x => x.PenaltyTotal);
                    int faults = currentClassReports.SelectMany(x => x.Faults).Count();
                    stats.Add(new LineChartStat
                    {
                        ClassId = classId,
                        Faults = faults,
                        PenaltyPoint = penaltyPoints
                    });
                }

                result.items.Add(dayofWeeks[dayofWeek], stats);
            }


            return result;
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
                ws.Cell(index, 6).SetValue(stat.TotalPoints);

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
    }

    internal class StatisticsQueryInput
    {
        public int StartPoints { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Weeks { get; set; }
    }
}
