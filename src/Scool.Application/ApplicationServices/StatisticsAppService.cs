using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Application.Permissions;
using Scool.Domain.Common;
using Scool.Domain.Views;
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
                SELECT 
                X.*, 
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
                WHERE (DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0) OR D.Id IS NULL
                GROUP BY A.ID
                ) X JOIN [AppClass] F ON F.ID = X.ClassId
                JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId
            ");

            var items = await query.ToListAsync();

            return new PagingModel<DcpClassFault>(items, items.Count);
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<CommonDcpFault>> GetCommonFaults(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter, false);

            var query = _context.CommonDcpFaults.FromSqlRaw(@$"
                SELECT X.*, Y.DisplayName Name, Z.DisplayName CriteriaName FROM
                (
                SELECT A.Id, COUNT(B.Id) Faults FROM [AppRegulation] A
                LEFT JOIN [AppDcpClassReportItem] B ON A.Id = B.RegulationId
                LEFT JOIN [AppDcpClassReport] C ON B.DcpClassReportId = C.Id
                LEFT JOIN [AppDcpReport] D ON C.DcpReportId = D.Id
                LEFT JOIN [AppClass] E ON C.ClassId = E.Id
                WHERE (DATEDIFF(DAY, '{input.StartTime}', D.CreationTime) >= 0 AND DATEDIFF(DAY, D.CreationTime, '{input.EndTime}') >= 0) OR B.Id IS NULL
                GROUP BY A.Id
                ) X JOIN [AppRegulation] Y ON X.Id = Y.Id
                JOIN [AppCriteria] Z ON Y.CriteriaId = Z.Id
                ORDER BY X.Faults DESC
            ");

            var items = await query.ToListAsync();

            return new PagingModel<CommonDcpFault>(items, items.Count);
        }

        [Authorize(StatsPermissions.Rankings)]
        public async Task<PagingModel<DcpClassRanking>> GetDcpRanking(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter);

            var query = _context.DcpClassRankings.FromSqlRaw(@$"
                SELECT
                ROW_NUMBER() OVER(ORDER BY X.TotalPoints DESC, X.Faults ASC) as Ranking,
                X.*, 
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
                    WHERE (DATEDIFF(DAY, '{input.StartTime}', C.CreationTime) >= 0 AND DATEDIFF(DAY, C.CreationTime, '{input.EndTime}') >= 0) OR D.Id IS NULL
                    GROUP BY A.ID
                ) X JOIN [AppClass] F ON F.ID = X.ClassId
                JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId
            ");

            var items = await query.ToListAsync();

            return new PagingModel<DcpClassRanking>(items, items.Count);
        }

        [Authorize(StatsPermissions.Statistics)]
        public async Task<PagingModel<StudentWithMostFaults>> GetStudentsWithMostFaults(TimeFilterDto timeFilter)
        {
            var input = ParseQueryInput(timeFilter, false);

            var query = _context.StudentWithMostFaults.FromSqlRaw(@$"
                SELECT X.*, Y.Name StudentName, Z.Name ClassName FROM
                (
                SELECT A.ID, COUNT(C.Id) Faults 
                FROM [AppStudent] A
                LEFT JOIN [AppDcpStudentReport] B ON A.Id = B.StudentId
                LEFT JOIN [AppDcpClassReportItem] C ON B.DcpClassReportItemId = C.Id
                LEFT JOIN [AppDcpClassReport] D ON C.DcpClassReportId = D.Id
                LEFT JOIN [AppDcpReport] E ON D.DcpReportId = E.Id
                WHERE (DATEDIFF(DAY, '{input.StartTime}', E.CreationTime) >= 0 AND DATEDIFF(DAY, E.CreationTime, '{input.EndTime}') >= 0) OR B.Id IS NULL
                GROUP BY A.Id
                ) X JOIN [AppStudent] Y ON X.Id = Y.Id
                JOIN [AppClass] Z ON Y.ClassId = Z.Id
                ORDER BY Faults DESC
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

        private StatisticsQueryInput ParseQueryInput(TimeFilterDto timeFilter, bool byWeek = true)
        {
            var startDate = byWeek && timeFilter.StartTime.DayOfWeek != DayOfWeek.Monday ?
                timeFilter.StartTime.AddDays(7).StartOfWeek() : timeFilter.StartTime;
            var durations = timeFilter.EndTime - startDate;
            var days = (int)Math.Ceiling(durations.TotalDays / 7);
            if (days == 0)
            {
                days = 1;
            }

            return new StatisticsQueryInput
            {
                StartPoints = days * 100,
                StartTime = startDate.ToString("MM/dd/yyyy"),
                EndTime = timeFilter.EndTime.ToString("MM/dd/yyyy")
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

        private static XLWorkbook GenerateTemplate(List<DcpClassRanking> stats, TimeFilterDto timeFilter)
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
    }
}
