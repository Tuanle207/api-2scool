using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;
using Scool.Users;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Identity;

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

            CreateMap<IdentityUser, UserDto>()
                .ConvertUsing(src => MapIdentityUserToUserDto(src));

            CreateMap<IdentityRole, RoleForSimpleListDto>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Name));
        }

        private UserDto MapIdentityUserToUserDto(IdentityUser src)
        {
            var result = new UserDto
            {
                Id = src.Id,
                Name = src.Name,
                UserName = src.UserName,
                Email = src.Email,
                EmailConfirmed = src.EmailConfirmed,
                PhoneNumber = src.PhoneNumber,
                PhoneNumberConfirmed = src.PhoneNumberConfirmed,
                ListRoleId = src.Roles.Select(x => x.RoleId).ToList(),
                Roles = new List<RoleForSimpleListDto>(),
                ConcurrencyStamp = src.ConcurrencyStamp
            };

            return result;
        }
    }
}
