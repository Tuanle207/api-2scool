using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;
using Scool.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.AutoMapperProfiles
{
    public class UsersAutoMapperProfile : Profile
    {
        public UsersAutoMapperProfile()
        {
            CreateMap<AppUser, UserForSimpleListDto>();
            CreateMap<UserProfile, UserForTaskAssignmentDto>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.PhoneNumber, opt =>
                    opt.MapFrom(dest => dest.PhoneNo))
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserProfileId, opt =>
                    opt.MapFrom(src => src.Id));
        }
    }
}
