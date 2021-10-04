using System;

namespace Scool.Domain.Views
{
    public class DcpClassRanking
    {
        public long Ranking { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public string FormTeacherName { get; set; }
        public int Faults { get; set; }
        public int PenaltyPoints { get; set; }
        public int TotalPoints { get; set; }
    }
}
