using Scool.RealTime.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Scool.RealTime
{
    public abstract class RealTimeController : AbpController
    {
        protected RealTimeController()
        {
            LocalizationResource = typeof(RealTimeResource);
        }
    }
}
