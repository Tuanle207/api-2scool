using Scool.Localization;
using Scool.Permission;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;


namespace Scool.Permission
{
    public class StatsPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(PermissionGroupsConst.Stats);
            string prefix = $"Permission:{PermissionGroupsConst.Stats}";

            if (group == null)
            {
                group = context.AddGroup(PermissionGroupsConst.Stats, L(prefix));
            }

            prefix += ":";

            group.AddPermission(StatsPermissions.Rankings,
                L(prefix + StatsPermissions.Rankings));

            group.AddPermission(StatsPermissions.Statistics,
                L(prefix + StatsPermissions.Statistics));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }
}
