using System;

namespace Scool.Application.Dtos
{
    public class CreateUpdateCriteriaDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Point { get; set; }
        public Guid CriteriaId { get; set; }
    }
}
