﻿
namespace Scool.Permission
{
    public static class IdentityPermissions
    {
        public const string GroupName = "AbpIdentity";

        public static class Roles
        {
            public const string Default = GroupName + ".Roles";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string ManagePermissions = Default + ".ManagePermissions";
        }

        public static class Users
        {
            public const string Default = GroupName + ".Users";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string Get = Default + ".Get";
            public const string ManagePermissions = Default + ".ManagePermissions";
        }

        public static class UserLookup
        {
            public const string Default = GroupName + ".UserLookup";
        }
    }
}
