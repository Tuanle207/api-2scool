using Scool.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;


namespace Scool.Application.Permissions
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

    public static class StatsPermissions
    {
        public const string Rankings = "Rankings";
        public const string Statistics = "Statistics";

    }
}
