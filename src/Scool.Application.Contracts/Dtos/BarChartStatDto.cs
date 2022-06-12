using System.Collections.Generic;

namespace Scool.Dtos
{
    public class BarChartStatDto
    {
        public List<BarChartStat> Items { get; set; }
    }

    public class BarChartStat
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public int Faults { get; set; }
    }
}
