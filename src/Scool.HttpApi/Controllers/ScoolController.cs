using Scool.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Scool.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class ScoolController : AbpController
    {
        protected ScoolController()
        {
            LocalizationResource = typeof(ScoolResource);
        }
    }
}