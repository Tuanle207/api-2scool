using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class RegulationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public Guid CriteriaId { get; set; }
        public CriteriaDto Criteria { get; set; }
    }
}
