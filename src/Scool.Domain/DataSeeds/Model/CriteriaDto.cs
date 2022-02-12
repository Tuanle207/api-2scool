using System.Collections.Generic;

namespace Scool.DataSeeds.Model
{
    internal class CriteriaDto
    {
        public string CriteriaName { get; set; }
        public IEnumerable<RegulationDto> Items { get; set; }
    }
}