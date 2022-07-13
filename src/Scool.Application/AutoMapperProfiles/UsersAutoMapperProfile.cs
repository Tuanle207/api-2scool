using AutoMapper;
using Scool.Common;
using Scool.Dtos;
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
            CreateMap<Account, SimpleAccountDto>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.IsStudent, opt =>
                    opt.MapFrom(src => src.StudentId.HasValue))
                .ForMember(dest => dest.IsTeacher, opt =>
                    opt.MapFrom(src => src.TeacherId.HasValue));

            CreateMap<IdentityUser, UserDto>()
                .ConvertUsing(src => MapIdentityUserToUserDto(src));

            CreateMap<IdentityUser, IdentityUserUpdateDto>();

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
