using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class GradesAutoMapperProfile : Profile
    {
        public GradesAutoMapperProfile()
        {
            CreateMap<Grade, GradeDto>();
            CreateMap<Grade, GradeForSimpleListDto>();
            CreateMap<CreateUpdateGradeDto, Grade>();
        }
    }
}
