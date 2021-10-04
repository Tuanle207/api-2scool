using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
{
    public interface ILrReportAppService : IBasicCrudAppService<
            Guid,
            LRReportDto,
            LRReportDto,
            CreateUpdateLRReportDto,
            CreateUpdateLRReportDto
        >
    {
        Task PostAcceptAsync(DcpReportAcceptDto input);
        Task PostRejectAsync(Guid id);
        Task PostCancelAssessAsync(Guid id);
        Task<PagingModel<LRReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input);
        Task<PagingModel<LRReportDto>> PostGetReportsForApprovalAsync(PageInfoRequestDto input);
    }
}
