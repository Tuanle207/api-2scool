using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Class : Entity<Guid>
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid GradeId { get; set; }
        public Grade Grade { get; set; }
        public Guid FormTeacherId { get; set; }
        public Teacher FormTeacher { get; set; }
        public ICollection<Student> Students { get; set; }
        public int NoStudents { get; set; }

        public Class()
        {
            Students = new List<Student>();
        }
    }

}