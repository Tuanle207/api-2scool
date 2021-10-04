using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.Application.AutoMapperProfiles
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
