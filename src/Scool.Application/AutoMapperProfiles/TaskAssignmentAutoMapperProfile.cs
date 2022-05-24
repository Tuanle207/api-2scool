using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class TaskAssignmentAutoMapperProfile : Profile
    {
        public TaskAssignmentAutoMapperProfile()
        {
            CreateMap<TaskAssignment, TaskAssignmentDto>();
            CreateMap<TaskAssignment, TaskAssignmentForUpdateDto>();
        }
    }
}
