using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class OtherAutoMapperProfile : Profile
    {
        public OtherAutoMapperProfile()
        {
            CreateMap<AppSetting, AppSettingDto>();
        }
    }
}
