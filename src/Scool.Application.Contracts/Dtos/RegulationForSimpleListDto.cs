using System;
using Volo.Abp.Application.Dtos;

namespace Scool.Application.Dtos
{
    public class RegulationForSimpleListDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public int Point { get; set; }
        public Guid CriteriaId { get; set; }
        public CriteriaForSimpleListDto Criteria { get; set; }
    }
}
