using Scool.Application.Dtos;
using Scool.Domain.Views;
using Scool.Infrastructure.Common;
using System.IO;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
{
    public interface IStatisticsAppService
    {
        Task<PagingModel<DcpClassRanking>> GetDcpRanking(TimeFilterDto timeFilter);
        Task<PagingModel<DcpClassFault>> GetClassesFaults(TimeFilterDto timeFilter);
        Task<PagingModel<CommonDcpFault>> GetCommonFaults(TimeFilterDto timeFilter);
        Task<PagingModel<StudentWithMostFaults>> GetStudentsWithMostFaults(TimeFilterDto timeFilter);
        Task<MemoryStream> GetDcpRankingExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetClassesFaultsExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetCommonFaultsExcel(TimeFilterDto timeFilter);
        Task<MemoryStream> GetStudentsWithMostFaultsExcel(TimeFilterDto timeFilter);
    }
}
