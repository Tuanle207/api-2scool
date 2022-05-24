using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Dtos
{
    public class RegulationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public string Type { get; set; }
        public Guid CriteriaId { get; set; }
        public CriteriaDto Criteria { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
    }
}
