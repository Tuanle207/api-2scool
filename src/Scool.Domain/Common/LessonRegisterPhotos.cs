using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class LessonRegisterPhotos : Entity<Guid>, IMultiTenant
    {
        public Guid LessonRegisterId { get; set; }
        public string Photo { get; set; }
        public Guid? TenantId { get; set; }
    }
}
