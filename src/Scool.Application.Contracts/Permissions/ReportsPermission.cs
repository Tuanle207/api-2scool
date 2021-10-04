using Scool.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Scool.Application.Permissions
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

            dcpReportPermission.AddChild(ReportsPermissions.GetDcpReportDetailApproval,
                L(prefix + ReportsPermissions.GetDcpReportDetailApproval));

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

            lrReportPermission.AddChild(ReportsPermissions.GetLRReportDetailApproval,
                L(prefix + ReportsPermissions.GetLRReportDetailApproval));

            lrReportPermission.AddChild(ReportsPermissions.UpdateLRReport,
                L(prefix + ReportsPermissions.UpdateLRReport));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ScoolResource>(name);
        }
    }

    public static class ReportsPermissions
    {
        // Dcp Report
        public const string DcpReportPermission = "DcpReports";
        public const string CreateNewDcpReport = DcpReportPermission + ".CreateNewDcpReport";
        public const string GetDcpReportApprovalHistory = DcpReportPermission + ".GetDcpReportApprovalHistory";
        public const string DcpReportApproval = DcpReportPermission + ".DcpReportApproval";
        public const string GetMyDcpReport = DcpReportPermission + ".GetMyDcpReport";
        public const string RemoveDcpReport = DcpReportPermission + ".RemoveDcpReport";
        public const string GetDcpReportDetail = DcpReportPermission + ".GetDcpReportDetail";
        public const string GetDcpReportDetailApproval = DcpReportPermission + ".GetDcpReportDetailApproval";
        public const string UpdateDcpReport = DcpReportPermission + ".UpdateDcpReport";

        // Lession Registration Report
        public const string LRReportsPermission = "LRReports";
        public const string CreateNewLRReport = LRReportsPermission + ".CreateNewLRReport";
        public const string GetLRApprovalHistory = LRReportsPermission + ".GetLRApprovalHistory";
        public const string LRReportApproval = LRReportsPermission + ".LRReportApproval";
        public const string GetMyLRReport = LRReportsPermission + ".GetMyLRReport";
        public const string RemoveLRReport = LRReportsPermission + ".RemoveLRReport";
        public const string GetLRReportDetail = LRReportsPermission + ".GetLRReportDetail";
        public const string GetLRReportDetailApproval = LRReportsPermission + ".GetLRReportDetailApproval";
        public const string UpdateLRReport = LRReportsPermission + ".UpdateLRReport";

    }
}