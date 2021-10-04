using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.Application.AutoMapperProfiles
{
    public class ClassesAutoMapperProfile : Profile
    {
        public ClassesAutoMapperProfile()
        {
            CreateMap<Class, ClassDto>();
            CreateMap<Class, ClassForListDto>();
            CreateMap<Class, ClassForSimpleListDto>();
            CreateMap<Class, ClassForStudentDto>();
            CreateMap<CreateUpdateClassDto, Class>();
        }
    }
}
