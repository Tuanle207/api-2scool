using System.Collections.Generic;

namespace Scool.DataSeeds.Model
{
    internal class CriteriaDto
    {
        internal CriteriaDto(string criteriaName)
        {
            CriteriaName = criteriaName;

        }
        public string CriteriaName { get; set; }
        public IEnumerable<RegulationDto> Items { get; set; }
    }
}