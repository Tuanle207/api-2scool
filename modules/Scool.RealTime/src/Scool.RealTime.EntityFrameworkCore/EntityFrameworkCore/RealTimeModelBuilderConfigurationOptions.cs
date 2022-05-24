using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Scool.RealTime.EntityFrameworkCore
{
    public class RealTimeModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public RealTimeModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}