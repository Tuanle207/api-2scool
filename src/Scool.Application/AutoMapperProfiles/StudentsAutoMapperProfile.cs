using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.Application.AutoMapperProfiles
{
    public class StudentsAutoMapperProfile : Profile
    {
        public StudentsAutoMapperProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<Student, StudentForSimpleListDto>();
            CreateMap<CreateUpdateStudentDto, Student>();
            
        }
    }
}
