using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Application.Dtos
{
    public class CriteriaForSimpleListDto : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
