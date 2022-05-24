using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class NotificationDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Seen { get; set; } = false;
        public DateTime CreationTime { get; set; }
        public SimpleAccountDto FromAccount { get; set; }
    }
}
