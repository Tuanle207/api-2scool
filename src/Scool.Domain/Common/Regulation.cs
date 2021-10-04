using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Regulation : Entity<Guid>
    {
        public string Name { get; set; }
        public string DisplayName  { get; set; }
        public int Point { get; set; }
        public Guid CriteriaId { get; set; }
        public Criteria Criteria { get; set; }
    }
}