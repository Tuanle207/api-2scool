using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class LessonRegisterPhotos : Entity<Guid>
    {
        public Guid LessonRegisterId { get; set; }
        public string Photo { get; set; }
    }
}
