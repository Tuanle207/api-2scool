using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Domain.Common;
using Scool.Infrastructure.ApplicationServices;
using Scool.Infrastructure.Common;
using Scool.Infrastructure.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Scool.Application.ApplicationServices
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

        public override async Task<PagingModel<StudentDto>> PostPagingAsync(PageInfoRequestDto input)
        {
            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var query = _studentRepo.Filter(input.Filter);
            query = string.IsNullOrEmpty(input.SortName) ? query.OrderBy(x => x.Id) : query.OrderBy(input.SortName, input.Ascend);
            query = query.Page(pageIndex, pageSize);
            query = query.Include(e => e.Class);

            var items = ObjectMapper.Map<List<Student>, List<StudentDto>>(await query.ToListAsync());
            var totalCount = await _studentRepo.Filter(input.Filter).CountAsync();

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
        public async Task<PagingModel<StudentForSimpleListDto>> GetSimpleListAsync([FromQuery(Name = "classId")]Guid? classId)
        {
            var items = await _studentRepo
                .WhereIf(classId != null, x => x.ClassId == (Guid)classId)
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
