using System;

namespace Scool.Domain.Views
{
    public class CommonDcpFault
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CriteriaName { get; set; }
        public int Faults { get; set; }
    }
}
