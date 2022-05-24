using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class Account : Entity<Guid>, ICreationAuditedObject, ISoftDelete, IMultiTenant
    {
        public Account()
        {

        }

        public Account(bool nullAccount = false)
        {
            if (nullAccount)
            {
                Id = Guid.Empty;
            }
        }

        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Dob { get; set; }
        public string Avatar { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public bool IsDeleted { get; set; }
        public string ClassDisplayName { get; set; } //TODO: migrate DB
        public Class Class { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public Guid? TenantId { get; set; }
    }
}
