using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
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
