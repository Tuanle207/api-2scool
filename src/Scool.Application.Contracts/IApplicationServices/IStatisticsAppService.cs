using Scool.Dtos;
using Scool.Infrastructure.Common;
using Scool.Views;
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
    }
}
