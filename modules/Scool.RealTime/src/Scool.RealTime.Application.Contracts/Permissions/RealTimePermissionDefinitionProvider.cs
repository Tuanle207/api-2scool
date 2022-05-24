using Scool.RealTime.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.RealTime.Permissions
{
    public class RealTimePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(RealTimePermissions.GroupName, L("Permission:RealTime"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<RealTimeResource>(name);
        }
    }
}