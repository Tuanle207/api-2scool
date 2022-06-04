using System;

namespace Scool.Views
{
    public class LrClassRanking
    {
        public long Ranking { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public string FormTeacherName { get; set; }
        public int AbsenceNo { get; set; }
        public int TotalPoints { get; set; }
    }
}
