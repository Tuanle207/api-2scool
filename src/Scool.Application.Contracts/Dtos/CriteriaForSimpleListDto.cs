using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Dtos
{
    public class CriteriaForSimpleListDto : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
