using System;

namespace Scool.Dtos
{
    public class CreateUpdateRegulationDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }
        public Guid CriteriaId { get; set; }
        public string Type { get; set; }
    }
}
