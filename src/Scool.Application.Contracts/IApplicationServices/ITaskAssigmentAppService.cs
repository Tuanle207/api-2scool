using Scool.Application.Dtos;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Application.IApplicationServices
{
    public interface ITaskAssigmentAppService : IBasicCrudAppService<
            Guid,
            TaskAssignmentDto,
            TaskAssignmentDto,
            CreateUpdateTaskAssignmentDto,
            CreateUpdateTaskAssignmentDto
        >
    {
        Task CreateUpdateAsync(CreateUpdateTaskAssignmentDto input);
        Task<PagingModel<TaskAssignmentDto>> GetAllAsync(TaskAssignmentFilterDto input);
        Task<PagingModel<TaskAssignmentForUpdateDto>> GetForUpdateAsync(TaskAssignmentFilterDto input);
        Task<PagingModel<ClassForSimpleListDto>> GetAssignedClassesForDcpReportAsync(string taskType);
    }
}
