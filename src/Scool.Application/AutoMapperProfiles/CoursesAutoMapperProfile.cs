using AutoMapper;
using Scool.Application.Dtos;
using Scool.Domain.Common;

namespace Scool.Application.AutoMapperProfiles
{
    public class CoursesAutoMapperProfile : Profile
    {
        public CoursesAutoMapperProfile()
        {
            // CreateMap
            CreateMap<Course, CourseDto>();
            CreateMap<Course, CourseListDto>();
            CreateMap<CreateUpdateCourseDto, Course>();
            CreateMap<Course, CourseForSimpleListDto>();
        }
    }
}
