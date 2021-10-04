using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class LRReportDto : EntityDto<Guid>
    {
        public int TotalPoint { get; set; }
        public int AbsenceNo { get; set; }
        public ClassForSimpleListDto Class { get; set; }
        public List<string> AttachedPhotos { get; set; }
        public string Status { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
    }
}
