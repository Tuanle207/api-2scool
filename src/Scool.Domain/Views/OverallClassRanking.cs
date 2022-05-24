using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Views
{
    public class OverallClassRanking
    {
        public long Ranking { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public string FormTeacherName { get; set; }
        public int TotalAbsence { get; set; }
        public int Faults { get; set; }
        public int PenaltyPoints { get; set; }
        public int LrPoints { get; set; }
        public int DcpPoints { get; set; }
        public int RankingPoints { get; set; }
    }
}
