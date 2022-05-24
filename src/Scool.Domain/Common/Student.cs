using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Student : Entity<Guid>, IMultiTenant, ISoftDelete
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public DateTime Dob { get; set; }
        public string ParentPhoneNumber { get; set; }
        public Account Account { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsDeleted { get; set; }
    }
}







