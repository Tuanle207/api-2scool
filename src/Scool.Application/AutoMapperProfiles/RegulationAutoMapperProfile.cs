using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

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
