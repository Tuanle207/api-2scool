using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Scool.Application.Dtos;
using Scool.Application.IApplicationServices;
using Scool.Domain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Scool.ApplicationServices
{
    public class DataImportAppService : ApplicationService
    {
        private readonly ILogger<DataImportAppService> _logger;
        private readonly IRepository<Student, Guid> _studentRepository;
        private readonly IRepository<Class, Guid> _classRepository;
        private readonly IClassesAppService _classesAppService;

        public DataImportAppService(ILogger<DataImportAppService> logger, IRepository<Student, Guid> studentRepository,
            IRepository<Class, Guid> classRepository, IClassesAppService classesAppService)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _classesAppService = classesAppService;
        }

        public async Task PostImportStudentsData(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var wb = new XLWorkbook(stream);

                if (!wb.Worksheets.Any())
                {
                    return;
                }
                IXLWorksheet ws = wb.Worksheets.FirstOrDefault();

                if (ws.ColumnsUsed().Count() != 5)
                {
                    return;
                }

                int rowCount = ws.RowsUsed().Count();
                IList<StudentDataImportDto> students = new List<StudentDataImportDto>();

                for (var i = 2; i <= rowCount; i++)
                {
                    var index = ws.Cell(i, 1).GetValue<int>();
                    var fullName = ws.Cell(i, 2).GetString();
                    var className = ws.Cell(i, 3).GetString();
                    var dob = ws.Cell(i, 4).GetDateTime();
                    var parentPhoneNumber = ws.Cell(i, 5).GetString();

                    var student = new StudentDataImportDto
                    {
                        Index = index,
                        FullName = fullName,
                        ClassName = className,
                        Dob = dob,
                        ParentPhoneNumber = parentPhoneNumber
                    };

                    students.Add(student);
                }

                if (students.Any())
                {
                    var classes = (await _classesAppService.GetSimpleListAsync()).Items;
                    IList<Student> formatStudents = new List<Student>();

                    foreach (var student in students)
                    {
                        var formatStudent = new Student
                        {
                            Name = student.FullName,
                            Dob = student.Dob,
                            ParentPhoneNumber = student.ParentPhoneNumber,
                        };
                        var matchClass = classes.FirstOrDefault(c => c.Name == student.ClassName);
                        if (matchClass != null)
                        {
                            formatStudent.ClassId = matchClass.Id;
                            formatStudents.Add(formatStudent);
                        }
                    }
                    await _studentRepository.InsertManyAsync(formatStudents);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            } 
            catch (Exception exception)
            {
                _logger.LogError("Error when reading student data excel file: ", exception.Message, exception);
            }
        }
    }
}
