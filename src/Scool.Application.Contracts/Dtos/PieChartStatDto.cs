using System.Collections.Generic;

namespace Scool.Dtos
{
    public class PieChartStatDto
    {
        public List<PieChartStat> Items { get; set; }

    }

    public class PieChartStat
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
