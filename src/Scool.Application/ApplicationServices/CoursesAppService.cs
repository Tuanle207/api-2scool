using System;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using Scool.Infrastructure.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Scool.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scool.Dtos;
using Scool.Common;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Permission;
using Scool.AppConsts;

namespace Scool.ApplicationServices
{
    public class CoursesAppService :
        BasicCrudAppService<
            Course,
            Guid,
            CourseDto,
            CourseListDto,
            CreateUpdateCourseDto,
            CreateUpdateCourseDto
        >, ICoursesAppService
    {
        private readonly IRepository<Teacher, Guid> _teacherRepo;
        private readonly IRepository<Class, Guid> _classesRepo;
        private readonly IRepository<Student, Guid> _studentRepo;
        private readonly IRepository<Regulation, Guid> _regulationRepo;
        private readonly IRepository<Course, Guid> _courseRepo;
        private readonly IRepository<Grade, Guid> _gradesRepo;

        public CoursesAppService(IRepository<Course, Guid> courseRepo,
            IRepository<Teacher, Guid> teacherRepo,
            IRepository<Class, Guid> classesRepo,
            IRepository<Student, Guid> studentRepo,
            IRepository<Regulation, Guid> regulationRepo,
            IRepository<Grade, Guid> gradesRepo) : base(courseRepo)
        {
            GetPolicyName = CoursesPermissions.Get;
            GetListPolicyName = CoursesPermissions.GetAll;
            CreatePolicyName = CoursesPermissions.Create;
            UpdatePolicyName = CoursesPermissions.Update;
            DeletePolicyName = CoursesPermissions.Delete;
            _courseRepo = courseRepo;
            _teacherRepo = teacherRepo;
            _classesRepo = classesRepo;
            _studentRepo = studentRepo;
            _regulationRepo = regulationRepo;
            _gradesRepo = gradesRepo;
        }

        public override async Task<CourseDto> CreateAsync(CreateUpdateCourseDto input)
        {
            await CheckCreatePolicyAsync();

            var entity = await MapToEntityAsync(input);

            TryToSetTenantId(entity);

            await Repository.InsertAsync(entity);

            if (input.FromActiveCourse)
            {
                await CopyDataFromActiveCourse(entity.Id);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            return await MapToGetOutputDtoAsync(entity);
        }

        [Authorize(CoursesPermissions.GetAll)]
        public async Task<PagingModel<CourseForSimpleListDto>> GetSimpleListAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = _courseRepo.Filter(input.Filter)
                            .OrderBy(x => x.StartTime);

            var items = await query.Select(x => ObjectMapper.Map<Course, CourseForSimpleListDto>(x))
                .ToListAsync();
            var totalCount = await query.CountAsync();

            return new PagingModel<CourseForSimpleListDto>(items, totalCount, pageIndex, pageSize);
        }

        [Authorize(CoursesPermissions.GetAll)]
        [HttpGet("api/app/courses/is-name-already-used")]
        public async Task<bool> IsNameAlreadyUsedAsync([FromQuery] Guid? id, [FromQuery] string name)
        {
            var lowercaseName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            return await _courseRepo.AsNoTracking()
                .Where(x => x.Id != id && x.Name.ToLower() == lowercaseName)
                .AnyAsync();
        }

        [Authorize(CoursesPermissions.Update)]
        [HttpGet("api/app/courses/activate/{id}")]
        public async Task<bool> MarkAsActiveCourseAsync(Guid id)
        {
            var allCourses = await _courseRepo.ToListAsync();

            var activeCourses = allCourses.Where(x => x.IsActive).ToList();
            foreach (var activeCourse in activeCourses)
            {
                activeCourse.IsActive = false;
            }
            var newActiveCourse = allCourses.FirstOrDefault(x => x.Id == id);
            if (newActiveCourse == null)
            {
                return false;
            }
            newActiveCourse.IsActive = true;
            await _courseRepo.UpdateManyAsync(allCourses);
            await CurrentUnitOfWork.SaveChangesAsync();
            return true;
        }

        [HttpGet("api/app/courses/has-active-course")]
        public async Task<bool> HasActiveCourseAsync()
        {
            return await _courseRepo.AsNoTracking().AnyAsync(x => x.IsActive);
        }

        private async Task CopyDataFromActiveCourse(Guid courseId)
        {
            var activeCourse = await Repository.FirstOrDefaultAsync(x => x.IsActive);
            if (activeCourse == null)
            {
                return;
            }

            await CopyTeachersFromCourse(activeCourse.Id, courseId);
            await CopyStudentsAndClassesFromCourse(activeCourse.Id, courseId);
            await CopyRegulationsFromCourse(activeCourse.Id, courseId);
        }

        private async Task CopyTeachersFromCourse(Guid activeCourseId, Guid courseId)
        {
            var teachers = await _teacherRepo.AsNoTracking()
                .Where(x => x.CourseId == activeCourseId).ToListAsync();

            var newTeachers = teachers.Select(x => new Teacher
            {
                Name = x.Name,
                Dob = x.Dob,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                CourseId = courseId,
                TenantId = CurrentTenant.Id,
            }).ToList();

            await _teacherRepo.InsertManyAsync(newTeachers);
        }

        private async Task CopyStudentsAndClassesFromCourse(Guid activeCourseId, Guid courseId)
        {
            var grades = await _gradesRepo.AsNoTracking().ToListAsync();
            var classes = await _classesRepo.AsNoTracking()
                .Where(x => x.CourseId == activeCourseId).ToListAsync();

            var students = await _studentRepo.AsNoTracking()
                .Where(x => x.CourseId == activeCourseId).ToListAsync();

            var grade10 = grades.FirstOrDefault(x => x.GradeCode == GradeCode.Ten);
            var grade11 = grades.FirstOrDefault(x => x.GradeCode == GradeCode.Eleven);
            var grade12 = grades.FirstOrDefault(x => x.GradeCode == GradeCode.Twelve);

            var newClasses = classes.Where(x => x.Grade == grade10 || x.Grade == grade11)
                .Select(x => new Class
                {
                    Name = GetNewClassName(x.Name),
                    GradeId = x.Grade == grade10 ? grade11.Id : grade12.Id,
                    NoStudents = x.NoStudents,
                    FormTeacherId = null,
                    CourseId = courseId,
                    TenantId = CurrentTenant.Id,
                }).ToList();

            var newStudents = students.Select(x => new Student
            {
                Name = x.Name,
                CourseId = courseId,
                TenantId = CurrentTenant.Id,
                ParentPhoneNumber = x.ParentPhoneNumber,
                Dob = x.Dob,
                Class = newClasses.FirstOrDefault(c => c.Name == x.Class.Name),
            });

            await _classesRepo.InsertManyAsync(newClasses);
            await _studentRepo.InsertManyAsync(newStudents);
        }

        private async Task CopyRegulationsFromCourse(Guid activeCourseId, Guid courseId)
        {
            var regulations = await _regulationRepo.AsNoTracking()
                .Where(x => x.CourseId == activeCourseId).ToListAsync();

            var newRegulations = regulations.Select(x => new Regulation
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                CreationTime = x.CreationTime,
                CreatorId = x.CreatorId,
                CriteriaId = x.CriteriaId,
                IsActive = x.IsActive,
                Point = x.Point,
                Type = x.Type,
                CourseId = courseId,
                TenantId = CurrentTenant.Id,
            }).ToList();

            await _regulationRepo.InsertManyAsync(newRegulations);
        }

        private string GetNewClassName(string clsname)
        {
            if (clsname.Contains("10"))
            {
                return clsname.Replace("10", "11");
            }
            if (clsname.Contains("11"))
            {
                return clsname.Replace("11", "12");
            }
            return clsname;
        }
    }
}