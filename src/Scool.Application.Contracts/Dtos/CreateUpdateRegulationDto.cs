using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateRegulationDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Point { get; set; }
        public Guid CriteriaId { get; set; }
    }
}
