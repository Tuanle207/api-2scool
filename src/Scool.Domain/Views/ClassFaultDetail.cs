using System;

namespace Scool.Views
{
    public class ClassFaultDetail
    {
        public Guid Id { get; set; }
        public Guid RegulationId { get; set; }
        public string RegulationName { get; set; }
        public string CriteriaName { get; set; }
        public int PenaltyPoints { get; set; }
        public DateTime CreationTime { get; set; }
        public string StudentNames { get; set; }
    }
}
