using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Student : Entity<Guid>, IMultiTenant, IHaveCourse, ISoftDelete
    {
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public Course Course{ get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public DateTime Dob { get; set; }
        public string ParentPhoneNumber { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsDeleted { get; set; }
    }
}







