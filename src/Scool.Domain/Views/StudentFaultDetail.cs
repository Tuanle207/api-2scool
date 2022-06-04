using System;

namespace Scool.Views
{
    public class StudentFaultDetail
    {
        public Guid Id { get; set; }
        public string RegulationName { get; set; }
        public string CriteriaName { get; set; }
        public int PenaltyPoints { get; set; }
        public int Count { get; set; } // count the number of faults
        public DateTime? CreationTime { get; set; } // dont count the number of faults
    }
}
