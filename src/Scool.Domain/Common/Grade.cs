using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Grade : Entity<Guid>
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}