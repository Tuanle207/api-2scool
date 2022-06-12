using Scool.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Permission
{
    internal class RulesPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(PermissionGroupsConst.Rules);
            string prefix = $"Permission:{PermissionGroupsConst.Rules}";

            if (group == null)
            {
                group = context.AddGroup(PermissionGroupsConst.Rules, L(prefix));
            }

            prefix += ":";

            var coursePermission = group.AddPermission(RulesPermissions.Permission,
                L(prefix + RulesPermissions.Permission));

            coursePermission.AddChild(RulesPermissions.Get,
                L(prefix + RulesPermissions.Get));

            coursePermission.AddChild(RulesPermissions.GetAll,
                L(prefix + RulesPermissions.GetAll));

            coursePermission.AddChild(RulesPermissions.Create,
                L(prefix + RulesPermissions.Create));

            coursePermission.AddChild(RulesPermissions.Update,
                L(prefix + RulesPermissions.Update));

            coursePermission.AddChild(RulesPermissions.Delete,
                L(prefix + RulesPermissions.Delete));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }
}
