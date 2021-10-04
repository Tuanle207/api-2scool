using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.AutoMapperProfiles
{
    public class TaskAssignmentAutoMapperProfile : Profile
    {
        public TaskAssignmentAutoMapperProfile()
        {
            CreateMap<TaskAssignment, TaskAssignmentDto>()
                 .ForMember(dest => dest.Assignee, opt =>
                     opt.MapFrom(src => src.AssigneeProfile))
                 .ForMember(dest => dest.ClassAssigned, opt =>
                     opt.MapFrom(src => src.ClassAssigned));

            CreateMap<TaskAssignment, TaskAssignmentForUpdateDto>();

            CreateMap<UserProfile, UserProfleForTaskAssignmentDto>()
                .ForMember(dest => dest.Name, opt =>
                     opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.PhoneNumber, opt =>
                     opt.MapFrom(src => src.PhoneNo))
                .ForMember(dest => dest.Name, opt =>
                     opt.MapFrom(src => src.DisplayName));
        }
    }
}
