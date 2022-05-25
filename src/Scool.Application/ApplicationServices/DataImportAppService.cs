﻿using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Scool.Common;
using Scool.Dtos;
using Scool.IApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRepository<Teacher, Guid> _teacherRepository;
        private readonly IRepository<Regulation, Guid> _regulationRepository;
        private readonly IRepository<Criteria, Guid> _criteriasRepository;
        private readonly IRepository<Course, Guid> _coursesRepository;
        private readonly IClassesAppService _classesAppService;
        private readonly IGradesAppService _gradesAppService;
        private readonly ITeachersAppService _teacherAppService;

        public DataImportAppService(
            ILogger<DataImportAppService> logger,
            IRepository<Student, Guid> studentRepository,
            IRepository<Class, Guid> classRepository,
            IRepository<Teacher, Guid> teacherRepository,
            IRepository<Regulation, Guid> regulationRepository,
            IRepository<Criteria, Guid> criteriasRepository,
            IRepository<Course, Guid> coursesRepository,
            IClassesAppService classesAppService,
            IGradesAppService gradesAppService,
            ITeachersAppService teacherAppService)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
            _regulationRepository = regulationRepository;
            _coursesRepository = coursesRepository;
            _classesAppService = classesAppService;
            _gradesAppService = gradesAppService;
            _teacherAppService = teacherAppService;
            _criteriasRepository = criteriasRepository;
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
                var students = new List<StudentDataImportDto>();

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
                        ParentPhoneNumber = parentPhoneNumber,
                    };

                    students.Add(student);
                }

                if (students.Any())
                {
                    var classes = (await _classesAppService.GetSimpleListAsync()).Items;
                    var formatStudents = new List<Student>();

                    foreach (var student in students)
                    {
                        var formatStudent = new Student
                        {
                            Name = student.FullName,
                            Dob = student.Dob,
                            ParentPhoneNumber = student.ParentPhoneNumber,
                            TenantId = CurrentTenant.Id
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

        public async Task PostImportTeachersData(IFormFile file)
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
                var teachers = new List<TeacherDataImportDto>();

                for (var i = 2; i <= rowCount; i++)
                {
                    var index = ws.Cell(i, 1).GetValue<int>();
                    var fullName = ws.Cell(i, 2).GetString();
                    var dob = ws.Cell(i, 3).GetDateTime();
                    var email = ws.Cell(i, 4).GetString();
                    var phoneNumber = ws.Cell(i, 5).GetString();

                    var teacher = new TeacherDataImportDto
                    {
                        Index = index,
                        FullName = fullName,
                        Dob = dob,
                        Email = email,
                        PhoneNumber = phoneNumber
                    };

                    teachers.Add(teacher);
                }

                if (teachers.Any())
                {
                    var formatTeachers = new List<Teacher>();

                    foreach (var teacher in teachers)
                    {
                        var formatTeacher = new Teacher
                        {
                            Name = teacher.FullName,
                            Dob = teacher.Dob,
                            Email = teacher.Email,
                            PhoneNumber = teacher.PhoneNumber,
                            TenantId = CurrentTenant.Id
                        };
                        formatTeachers.Add(formatTeacher);
                    }
                    await _teacherRepository.InsertManyAsync(formatTeachers);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Error when reading teacher data excel file: ", exception.Message, exception);
            }
        }

        public async Task PostImportClassesData(IFormFile file)
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

                if (ws.ColumnsUsed().Count() != 4)
                {
                    return;
                }

                int rowCount = ws.RowsUsed().Count();
                var classes = new List<ClassDataImportDto>();

                for (var i = 2; i <= rowCount; i++)
                {
                    var index = ws.Cell(i, 1).GetValue<int>();
                    var name = ws.Cell(i, 2).GetString();
                    var gradeName = ws.Cell(i, 3).GetString();
                    var formTeacherName = ws.Cell(i, 4).GetString();

                    var classItem = new ClassDataImportDto
                    {
                        Index = index,
                        Name = name,
                        GradeName = gradeName,
                        FormTeacherName = formTeacherName
                    };

                    classes.Add(classItem);
                }

                if (classes.Any())
                {
                    var formatClasses = new List<Class>();
                    var grades = (await _gradesAppService.GetSimpleListAsync()).Items;
                    var teachers = (await _teacherAppService.GetSimpleListAsync()).Items;
                    var course = _coursesRepository.OrderByDescending(x => x.StartTime)
                        .FirstOrDefault();

                    if (course is null)
                    {
                        return;
                    }
                    foreach (var cls in classes)
                    {
                        var formatClass = new Class
                        {
                            Name = cls.Name,
                            CourseId = course.Id,
                            TenantId = CurrentTenant.Id
                        };
                        var grade = grades.FirstOrDefault(x => x.DisplayName == cls.GradeName);
                        var teacher = teachers.FirstOrDefault(x => x.Name == cls.FormTeacherName);
                        if (grade is null || teacher is null) { }
                        {
                            formatClass.GradeId = grade.Id;
                            formatClass.FormTeacherId = teacher.Id;
                            formatClasses.Add(formatClass);
                        }
                    }
                    await _classRepository.InsertManyAsync(formatClasses);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Error when reading teacher data excel file: ", exception.Message, exception);
            }
        }

        public async Task PostImportRegulationsData(IFormFile file)
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
                var regulations = new List<RegulationDataImportDto>();

                for (var i = 2; i <= rowCount; i++)
                {
                    var index = ws.Cell(i, 1).GetValue<int>();
                    var name = ws.Cell(i, 2).GetString();
                    var point = ws.Cell(i, 3).GetValue<int>();
                    var type = ws.Cell(i, 4).GetString();
                    var criteriaName = ws.Cell(i, 5).GetString();

                    var regulation = new RegulationDataImportDto
                    {
                        Index = index,
                        Name = name,
                        Point = point,
                        Type = type,
                        CriteriaName = criteriaName
                    };

                    regulations.Add(regulation);
                }

                if (regulations.Any())
                {
                    var formatRegulations = new List<Regulation>();
                    var critierias = await _criteriasRepository.ToListAsync();
                    var newCriterias = new List<Criteria>();
                    foreach (var regulation in regulations)
                    {
                        var formatRegulation  = new Regulation
                        {
                            DisplayName = regulation.Name,
                            Point = regulation.Point,
                            Type = RegulationDataImportDto.GetRegulationType(regulation.Type),
                            TenantId = CurrentTenant.Id,
                        };
                        var criteria = critierias.FirstOrDefault(x => x.DisplayName == regulation.CriteriaName);
                        if (criteria is null)
                        {
                            criteria = new Criteria
                            {
                                DisplayName = regulation.CriteriaName,
                                TenantId = CurrentTenant.Id
                            };
                            newCriterias.Add(criteria);
                        }
                        formatRegulation.Criteria = criteria;
                        formatRegulations.Add(formatRegulation);
                    }
                    if (newCriterias.Any())
                    {
                        await _criteriasRepository.InsertManyAsync(newCriterias);
                    }
                    await _regulationRepository.InsertManyAsync(formatRegulations);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Error when reading teacher data excel file: ", exception.Message, exception);
            }
        }
    }
}
