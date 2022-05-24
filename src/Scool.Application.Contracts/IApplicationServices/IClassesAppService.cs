using Scool.Dtos;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using System;
using System.Threading.Tasks;

namespace Scool.IApplicationServices
{
    public interface IClassesAppService : IBasicCrudAppService<
        Guid,
        ClassDto,
        ClassForListDto,
        CreateUpdateClassDto,
        CreateUpdateClassDto
    >
    {
        Task<PagingModel<ClassForSimpleListDto>> GetSimpleListAsync();
    }
}
