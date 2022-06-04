using Scool.AppConsts;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Scool.Common
{
    public class LessonsRegister : Entity<Guid>, IHaveCourse, IHaveCreationInfo, IMultiTenant
    {
        public int TotalPoint { get; set; }
        public int AbsenceNo { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public string ReportedClassDisplayName { get; set; }
        public string Status { get; set; }
        public ICollection<LessonRegisterPhotos> AttachedPhotos { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Account CreatorAccount { get; set; }
        public Guid? TenantId { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public LessonsRegister()
        {
            Status = DcpReportStatus.Created;
            AttachedPhotos = new List<LessonRegisterPhotos>();
        }
    }
}