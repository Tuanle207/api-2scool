﻿using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IDcpReportsAppService : IBasicCrudAppService<
        Guid,
        DcpReportDto,
        DcpReportDto,
        CreateUpdateDcpReportDto,
        CreateUpdateDcpReportDto
    >
    {
        Task PostAcceptAsync(DcpReportAcceptDto input);
        Task PostRejectAsync(Guid id);
        Task PostCancelAssessAsync(Guid id);
        Task<PagingModel<DcpReportDto>> PostGetMyReportsAsync(PageInfoRequestDto input);
        Task<PagingModel<DcpReportDto>> PostGetReportsForApprovalAsync(PageInfoRequestDto input);
        Task<CreateUpdateDcpReportDto> GetUpdateAsync(Guid id);

    }
}
