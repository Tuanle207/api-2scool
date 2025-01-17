using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
namespace Scool.Common
{
    public class Teacher : Entity<Guid>, ISoftDelete, IMultiTenant, IHaveCourse
    {
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Class FormClass { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsDeleted { get; set; }
    }
}