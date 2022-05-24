using System;

namespace Scool.Common
{
    public interface IHaveCreationInfo
    {
        Guid? CreatorId { get; set; }
        Account CreatorAccount { get; set; }
        DateTime CreationTime { get; set; }
    }
}
