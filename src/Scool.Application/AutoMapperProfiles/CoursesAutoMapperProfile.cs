using AutoMapper;
using Scool.Common;
using Scool.Dtos;

namespace Scool.AutoMapperProfiles
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
