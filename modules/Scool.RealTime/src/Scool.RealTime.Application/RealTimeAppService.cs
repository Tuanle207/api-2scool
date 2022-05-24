using Scool.RealTime.Localization;
using Volo.Abp.Application.Services;

namespace Scool.RealTime
{
    public abstract class RealTimeAppService : ApplicationService
    {
        protected RealTimeAppService()
        {
            LocalizationResource = typeof(RealTimeResource);
            ObjectMapperContext = typeof(RealTimeApplicationModule);
        }
    }
}
