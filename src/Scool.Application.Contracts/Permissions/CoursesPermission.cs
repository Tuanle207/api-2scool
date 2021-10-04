using Scool.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Application.Permissions
{
    internal class CoursesPermission: PermissionDefinitionProvider
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
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }

    public static class CoursesPermissions
    {
        public const string Permission = "Courses";
        public const string Get = Permission + ".Get";
        public const string GetAll = Permission + ".GetAll";
        public const string Create = Permission + ".Create";
        public const string Update = Permission + ".Update";
        public const string Delete = Permission + ".Delete";
    }
}