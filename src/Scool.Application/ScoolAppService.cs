using System;
using System.Collections.Generic;
using System.Text;
using Scool.Localization;
using Volo.Abp.Application.Services;

namespace Scool
{
    /* Inherit your application services from this class.
     */
    public abstract class ScoolAppService : ApplicationService
    {
        protected ScoolAppService()
        {
            LocalizationResource = typeof(ScoolResource);
        }
    }
}
