using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scool.Dtos;
using Scool.Infrastructure.Common;
using Scool.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Scool.IApplicationServices
{
    public interface IStatisticsAppService
    {
        Task<PagingModel<OverallClassRanking>> GetOverallRanking(TimeFilterDto timeFilter);
        Task<PagingModel<DcpClassRanking>> GetDcpRanking(TimeFilterDto timeFilter);
        Task<PagingModel<LrClassRanking>> GetLrRanking(TimeFilterDto timeFilter);
        Task<PagingModel<DcpClassFault>> GetClassesFaults(TimeFilterDto timeFilter);
        Task<PagingModel<CommonDcpFault>> GetCommonFaults(TimeFilterDto timeFilter);
        Task<PagingModel<StudentWithMostFaults>> GetStudentsWithMostFaults(TimeFilterDto timeFilter);
        Task<LineChartStatDto> GetStatForLineChart(TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetOverallRankingExcel(TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetDcpRankingExcel(TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetClassesFaultsExcel(TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetCommonFaultsExcel(TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetStudentsWithMostFaultsExcel(TimeFilterDto timeFilter);
        Task<PagingModel<ClassFaultDetail>> GetClassFaultDetails(Guid classId, TimeFilterDto filter);
        Task<PagingModel<FaultDetail>> GetRegulationFaultDetails(Guid regulationId, TimeFilterDto filter);
        Task<PagingModel<StudentFaultDetail>> GetStudentFaultDetails(Guid studentId, TimeFilterDto filter);
        Task<IRemoteStreamContent> GetStudentFaultDetailsExcel([FromRoute(Name = "studentId")] Guid studentId, TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetRegulationFaultDetailsExcel([FromRoute(Name = "regulationId")] Guid regulationId, TimeFilterDto timeFilter);
        Task<IRemoteStreamContent> GetClassFaultDetailsExcel([FromRoute(Name = "classId")] Guid classId, TimeFilterDto timeFilter);
        Task SendClassFaultsThroughEmail(Guid? classId, DateTime inputStartTime, DateTime inputEndTime);
    }
}
