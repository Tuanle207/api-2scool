using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class RegulationAutoMapperProfile : Profile
    {
        public RegulationAutoMapperProfile()
        {
            CreateMap<Regulation, RegulationDto>();
            CreateMap<Regulation, RegulationForSimpleListDto>()
                .ForMember(dst => dst.Name, opt => 
                    opt.MapFrom(src => src.DisplayName));
            CreateMap<CreateUpdateRegulationDto, Regulation>();

        }
    }
}
