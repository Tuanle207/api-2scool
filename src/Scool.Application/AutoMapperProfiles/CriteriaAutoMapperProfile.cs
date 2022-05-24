using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class CriteriaAutoMapperProfile : Profile
    {
        public CriteriaAutoMapperProfile()
        {
            CreateMap<Criteria, CriteriaDto>();
            CreateMap<Criteria, CriteriaForSimpleListDto>()
                .ForMember(dst => dst.Name, opts => 
                    opts.MapFrom(src => src.DisplayName));
            CreateMap<CreateUpdateCriteriaDto, Criteria>();

        }
    }
}
