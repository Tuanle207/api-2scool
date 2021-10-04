using Volo.Abp.Settings;

namespace Scool.Settings
{
    public class ScoolSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(ScoolSettings.MySetting1));
        }
    }
}
