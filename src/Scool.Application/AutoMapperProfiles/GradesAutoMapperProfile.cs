using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.Application.AutoMapperProfiles
{
    public class GradesAutoMapperProfile : Profile
    {
        public GradesAutoMapperProfile()
        {
            CreateMap<Grade, GradeDto>();
            CreateMap<CreateUpdateGradeDto, Grade>();
        }
    }
}
