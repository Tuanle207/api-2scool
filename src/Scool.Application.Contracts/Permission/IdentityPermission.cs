using Scool.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Permission
{
    public class IdentityPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(IdentityPermissions.GroupName);
            string prefix = $"Permission:{IdentityPermissions.GroupName}";

            if (group == null)
            {
                group = context.AddGroup(IdentityPermissions.GroupName, L(prefix));
            }

            var innerGroup = group.GetPermissionOrNull(IdentityPermissions.Users.Default);
            if (innerGroup != null)
            {
                innerGroup.AddChild(IdentityPermissions.Users.Get,
                    L(prefix + ":" + IdentityPermissions.Users.Get));
            }
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }
}
