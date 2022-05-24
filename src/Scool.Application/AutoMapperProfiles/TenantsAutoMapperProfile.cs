using AutoMapper;
using Scool.Domain.Shared.AppConsts;
using Scool.Dtos;
using Tenant = Volo.Abp.TenantManagement.Tenant;

namespace Scool.AutoMapperProfiles
{
    public class TenantsAutoMapperProfile : Profile
    {
        public TenantsAutoMapperProfile()
        {
            CreateMap<Tenant, TenantDto>()
                .ForMember(dest => dest.DisplayName, opt => 
                    opt.MapFrom(src => src.ExtraProperties.ContainsKey(TenantSettingType.DisplayName) ? 
                        src.ExtraProperties[TenantSettingType.DisplayName] : string.Empty))
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.ExtraProperties.ContainsKey(TenantSettingType.DisplayName) ?
                        src.ExtraProperties[TenantSettingType.PhotoUrl] : string.Empty));
        }
    }
}
