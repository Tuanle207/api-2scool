using System;

namespace Scool.Common
{
    public interface IHaveUpdationInfo : IHaveCreationInfo
    {
        Guid? LastUpdatorId { get; set; }
        Account LastUpdatorAccount { get; set; }
        DateTime? LastUpdationTime { get; set; }
    }
}
