﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using Scool.Infrastructure.AppService;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class StudentsAppService : BasicCrudAppService<
        Student,
        Guid,
        StudentDto,
        StudentDto,
        CreateUpdateStudentDto,
        CreateUpdateStudentDto
    >, IStudentsAppService
    {
        private readonly IRepository<Student, Guid> _studentRepo;

        public StudentsAppService(IRepository<Student, Guid> studentRepo) : base(studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public override async Task<StudentDto> CreateAsync(CreateUpdateStudentDto input)
        {
            var newStudent = ObjectMapper.Map<CreateUpdateStudentDto, Student>(input);
            newStudent.CourseId = ActiveCourse.Id.Value;
            newStudent.TenantId = CurrentTenant.Id;
            await Repository.InsertAsync(newStudent);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<Student, StudentDto>(newStudent);
        }

        public override async Task<PagingModel<StudentDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<StudentDto>();
            }

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = Repository.AsNoTracking()
                .Filter(input.Filter)
                .Where(x => x.CourseId == ActiveCourse.Id.Value);
            var totalCount = await query.CountAsync();

            query = query.Include(e => e.Class);

            query = string.IsNullOrEmpty(input.SortName) ?
                query.OrderBy(x => x.Class.Name)
                .OrderBy(x => x.Name) :
                query.OrderBy(input.SortName, input.Ascend);

            query = query.Page(pageIndex, pageSize);

            var items = await query.Select(x => ObjectMapper.Map<Student, StudentDto>(x))
                .ToListAsync();

            return new PagingModel<StudentDto>(items, totalCount, pageIndex, pageSize);
        }

        public override async Task<StudentDto> GetAsync(Guid id)
        {
            var entity = await _studentRepo.Where(e => e.Id == id)
                   .Include(e => e.Class)
                   .FirstOrDefaultAsync();
            return ObjectMapper.Map<Student, StudentDto>(entity);
        }

        [HttpGet("/api/app/students/simple-list")]
        public async Task<PagingModel<StudentForSimpleListDto>> GetSimpleListAsync([FromQuery(Name = "classId")] Guid? classId)
        {
            if (!ActiveCourse.IsAvailable)
            {
                return new PagingModel<StudentForSimpleListDto>();
            }

            var items = await _studentRepo
                .WhereIf(classId.HasValue, x => x.ClassId == classId.Value)
                .WhereIf(!classId.HasValue, x => x.CourseId == ActiveCourse.Id.Value)
                .Select(x => ObjectMapper.Map<Student, StudentForSimpleListDto>(x))
                .ToListAsync();

            var result = new PagingModel<StudentForSimpleListDto>
            (
                items: items,
                totalCount: items.Count
            );

            return result;
        }

    }
}
