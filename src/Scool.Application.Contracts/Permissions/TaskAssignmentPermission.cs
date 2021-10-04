using Scool.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Application.Permissions
{
    public class TaskAssignmentPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(PermissionGroupsConst.TaskAssignment);
            string prefix = $"Permission:{PermissionGroupsConst.TaskAssignment}";

            if (group == null)
            {
                group = context.AddGroup(PermissionGroupsConst.TaskAssignment, L(prefix));
            }

            prefix += ":";


            group.AddPermission(TaskAssignmentPermissions.AssignDcpReport,
                L(prefix + TaskAssignmentPermissions.AssignDcpReport));

            group.AddPermission(TaskAssignmentPermissions.AssignLessonRegisterReport,
               L(prefix + TaskAssignmentPermissions.AssignLessonRegisterReport));

            group.AddPermission(TaskAssignmentPermissions.GetScheduleList,
               L(prefix + TaskAssignmentPermissions.GetScheduleList));

            group.AddPermission(TaskAssignmentPermissions.GetMyAssignedSchedule,
               L(prefix + TaskAssignmentPermissions.GetMyAssignedSchedule));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }

    public static class TaskAssignmentPermissions
    {
        public const string AssignDcpReport = "AssignDcpReport";
        public const string AssignLessonRegisterReport = "AssignLessonRegisterReport";
        public const string GetScheduleList = "GetScheduleList";
        public const string GetMyAssignedSchedule = "GetMyAssignedSchedule";

    }
}