using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
{
    public class TeachersAutoMapperProfile : Profile
    {
        public TeachersAutoMapperProfile()
        {
            CreateMap<Teacher, TeacherDto>();
            CreateMap<Teacher, TeacherForSimpleListDto>();
            CreateMap<CreateUpdateTeacherDto, Teacher>();
        }
    }
}
