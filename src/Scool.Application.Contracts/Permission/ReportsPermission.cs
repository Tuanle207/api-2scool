using Scool.Localization;
using Scool.Permission;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Permission
{
    internal class ReportsPermission : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            PermissionGroupDefinition group = context.GetGroupOrNull(PermissionGroupsConst.Dcp);
            string prefix = $"Permission:{PermissionGroupsConst.Dcp}";

            if (group == null)
            {
                group = context.AddGroup(PermissionGroupsConst.Dcp, L(prefix));
            }

            prefix += ":";

            // Dcp Report

            var dcpReportPermission = group.AddPermission(ReportsPermissions.DcpReportPermission,
                L(prefix + ReportsPermissions.DcpReportPermission));

            dcpReportPermission.AddChild(ReportsPermissions.CreateNewDcpReport,
                L(prefix + ReportsPermissions.CreateNewDcpReport));

            dcpReportPermission.AddChild(ReportsPermissions.GetDcpReportApprovalHistory,
                L(prefix + ReportsPermissions.GetDcpReportApprovalHistory));

            dcpReportPermission.AddChild(ReportsPermissions.DcpReportApproval,
                L(prefix + ReportsPermissions.DcpReportApproval));

            dcpReportPermission.AddChild(ReportsPermissions.GetMyDcpReport,
                L(prefix + ReportsPermissions.GetMyDcpReport));

            dcpReportPermission.AddChild(ReportsPermissions.RemoveDcpReport,
                L(prefix + ReportsPermissions.RemoveDcpReport));

            dcpReportPermission.AddChild(ReportsPermissions.GetDcpReportDetail,
                L(prefix + ReportsPermissions.GetDcpReportDetail));

            dcpReportPermission.AddChild(ReportsPermissions.UpdateDcpReport,
                L(prefix + ReportsPermissions.UpdateDcpReport));

            // LR Report
            var lrReportPermission = group.AddPermission(ReportsPermissions.LRReportsPermission,
                L(prefix + ReportsPermissions.LRReportsPermission));

            lrReportPermission.AddChild(ReportsPermissions.CreateNewLRReport,
                L(prefix + ReportsPermissions.CreateNewLRReport));

            lrReportPermission.AddChild(ReportsPermissions.GetLRApprovalHistory,
                L(prefix + ReportsPermissions.GetLRApprovalHistory));

            lrReportPermission.AddChild(ReportsPermissions.LRReportApproval,
                L(prefix + ReportsPermissions.LRReportApproval));

            lrReportPermission.AddChild(ReportsPermissions.GetMyLRReport,
                L(prefix + ReportsPermissions.GetMyLRReport));

            lrReportPermission.AddChild(ReportsPermissions.RemoveLRReport,
                L(prefix + ReportsPermissions.RemoveLRReport));

            lrReportPermission.AddChild(ReportsPermissions.GetLRReportDetail,
                L(prefix + ReportsPermissions.GetLRReportDetail));

            lrReportPermission.AddChild(ReportsPermissions.UpdateLRReport,
                L(prefix + ReportsPermissions.UpdateLRReport));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }
}