using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Criteria : Entity<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ICollection<Regulation> Regulations { get; set; }

        public Criteria()
        {
            Regulations = new List<Regulation>();
        }
    }
}