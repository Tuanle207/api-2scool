using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Dtos
{
    public class LineChartStatDto
    {
        public Dictionary<string, List<LineChartStat>> items { get; set; }
    }

    public class LineChartStat
    {
        public Guid ClassId { get; set; }
        public int PenaltyPoint { get; set; }
        public int Faults { get; set; }
    }
}
