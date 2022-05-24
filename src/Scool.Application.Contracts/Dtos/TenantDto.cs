using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class TenantDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreationTime { get; set; }
        public string PhotoUrl { get; set; }
    }
}
