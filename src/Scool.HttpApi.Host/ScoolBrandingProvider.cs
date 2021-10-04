using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Scool
{
    [Dependency(ReplaceServices = true)]
    public class ScoolBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Scool";
    }
}
