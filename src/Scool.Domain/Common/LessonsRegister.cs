using Scool.AppConsts;
using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;


namespace Scool.Domain.Common

{
    public class LessonsRegister : Entity<Guid>, ICreationAuditedObject
    {
        public int TotalPoint { get; set; }
        public int AbsenceNo { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public string Status { get; set; }
        public ICollection<LessonRegisterPhotos> AttachedPhotos { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }

        public LessonsRegister()
        {
            Status = DcpReportStatus.Approved;
            AttachedPhotos = new List<LessonRegisterPhotos>();
        }
    }
}