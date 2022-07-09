using Scool.Localization;
using Scool.Permission;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Permission
{
    internal class CoursesPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(PermissionGroupsConst.Courses);
            string prefix = $"Permission:{PermissionGroupsConst.Courses}";

            if (group == null)
            {
                group = context.AddGroup(PermissionGroupsConst.Courses, L(prefix));
            }

            prefix += ":";

            var coursePermission = group.AddPermission(CoursesPermissions.Permission,
                L(prefix + CoursesPermissions.Permission));

            coursePermission.AddChild(CoursesPermissions.Get,
                L(prefix + CoursesPermissions.Get));

            coursePermission.AddChild(CoursesPermissions.GetAll,
                L(prefix + CoursesPermissions.GetAll));

            coursePermission.AddChild(CoursesPermissions.Create,
                L(prefix + CoursesPermissions.Create));

            coursePermission.AddChild(CoursesPermissions.Update,
                L(prefix + CoursesPermissions.Update));

            coursePermission.AddChild(CoursesPermissions.Delete,
                L(prefix + CoursesPermissions.Delete));

            coursePermission.AddChild(CoursesPermissions.Students,
               L(prefix + CoursesPermissions.Students));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }
}