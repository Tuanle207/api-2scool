namespace Scool.Permission
{
    public class ReportsPermissions
    {
        // Dcp Report
        public const string DcpReportPermission = "DcpReports";
        public const string CreateNewDcpReport = DcpReportPermission + ".CreateNewDcpReport";
        public const string GetDcpReportApprovalHistory = DcpReportPermission + ".GetDcpReportApprovalHistory";
        public const string DcpReportApproval = DcpReportPermission + ".DcpReportApproval";
        public const string GetMyDcpReport = DcpReportPermission + ".GetMyDcpReport";
        public const string RemoveDcpReport = DcpReportPermission + ".RemoveDcpReport";
        public const string GetDcpReportDetail = DcpReportPermission + ".GetDcpReportDetail";
        public const string UpdateDcpReport = DcpReportPermission + ".UpdateDcpReport";

        // Lession Registration Report
        public const string LRReportsPermission = "LRReports";
        public const string CreateNewLRReport = LRReportsPermission + ".CreateNewLRReport";
        public const string GetLRApprovalHistory = LRReportsPermission + ".GetLRApprovalHistory";
        public const string LRReportApproval = LRReportsPermission + ".LRReportApproval";
        public const string GetMyLRReport = LRReportsPermission + ".GetMyLRReport";
        public const string RemoveLRReport = LRReportsPermission + ".RemoveLRReport";
        public const string GetLRReportDetail = LRReportsPermission + ".GetLRReportDetail";
        public const string UpdateLRReport = LRReportsPermission + ".UpdateLRReport";
    }
}
