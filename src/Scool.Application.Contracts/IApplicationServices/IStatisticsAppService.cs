﻿using Scool.Dtos;
using Scool.Infrastructure.Common;
using Scool.Views;
using System.IO;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IStatisticsAppService
    {
        Task<PagingModel<OverallClassRanking>> GetOverallRanking(TimeFilterDto timeFilter);
        Task<PagingModel<DcpClassRanking>> GetDcpRanking(TimeFilterDto timeFilter);
        Task<PagingModel<DcpClassFault>> GetClassesFaults(TimeFilterDto timeFilter);
        Task<PagingModel<CommonDcpFault>> GetCommonFaults(TimeFilterDto timeFilter);
        Task<PagingModel<StudentWithMostFaults>> GetStudentsWithMostFaults(TimeFilterDto timeFilter);
        Task<LineChartStatDto> GetStatForLineChart(TimeFilterDto timeFilter);
        Task<MemoryStream> GetOverallRankingExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetDcpRankingExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetClassesFaultsExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetCommonFaultsExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetStudentsWithMostFaultsExcel(TimeFilterDto timeFilter);
    }
}
