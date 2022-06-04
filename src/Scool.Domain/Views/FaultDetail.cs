using System;

namespace Scool.Views
{
    public class FaultDetail
    {
        public Guid Id { get; set; }
        public int PenaltyPoints { get; set; }
        public DateTime CreationTime { get; set; }
        public string StudentNames { get; set; }
        public string ClassName { get; set; }
    }
}
