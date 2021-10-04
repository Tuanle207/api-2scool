using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

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
