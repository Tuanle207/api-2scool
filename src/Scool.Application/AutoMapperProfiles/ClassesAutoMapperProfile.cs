using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
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
