using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class TaskAssignmentAutoMapperProfile : Profile
    {
        public TaskAssignmentAutoMapperProfile()
        {
            CreateMap<TaskAssignment, TaskAssignmentDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.CreatorAccount));
            CreateMap<TaskAssignment, TaskAssignmentForUpdateDto>();
        }
    }
}
