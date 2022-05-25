using Microsoft.Extensions.Logging;
using Scool.AppConsts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;
using System.Linq;
using Volo.Abp.PermissionManagement;
using Scool.Permission;

namespace Scool.DataSeeds
{
    public class RolesDataSeedContributor : DataSeedBase, IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<IdentityRole, Guid> _identityRoleRepository;
        private readonly IRepository<PermissionGrant, Guid> _permissionGrantRepository;
        private readonly ILogger<RolesDataSeedContributor> _logger;

        public RolesDataSeedContributor(
            ILogger<RolesDataSeedContributor> logger,
            IRepository<IdentityRole, Guid> identityRoleRepository,
            IRepository<PermissionGrant, Guid> permissionGrantRepository)
        {
            _logger = logger;
            _identityRoleRepository = identityRoleRepository;
            _permissionGrantRepository = permissionGrantRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!context.TenantId.HasValue)
            {
                return;
            }
            try
            {
                _logger.LogInformation("Start to seed roles and permissions");

                var roleNames = new List<string> {
                    AppRole.Admin,
                    AppRole.DcpManager,
                    AppRole.DcpReporterStudent,
                    AppRole.LessonsRegisterReporter
                };

                var currentRoles = await _identityRoleRepository.ToListAsync();
                
                // App Admin
                if (currentRoles.All(c => c.Name != AppRole.Admin))
                {
                    var adminRole = new IdentityRole(Guid.NewGuid(), AppRole.Admin, context.TenantId)
                    {
                        IsStatic = false,
                        IsDefault = false,
                        IsPublic = true,
                    };
                    var adminRolePermissionNames = new List<string>
                    {
                        CoursesPermissions.Permission,
                        CoursesPermissions.Get,
                        CoursesPermissions.GetAll,
                        CoursesPermissions.Update,
                        CoursesPermissions.Delete,
                        CoursesPermissions.Create,

                        IdentityPermissions.Users.Default,
                        IdentityPermissions.Users.Create,
                        IdentityPermissions.Users.Update,
                        IdentityPermissions.Users.Delete,
                        IdentityPermissions.Users.ManagePermissions,
                    };
                    var adminRolePermissions = adminRolePermissionNames
                        .Select(x => new PermissionGrant(
                            id: Guid.NewGuid(), 
                            name: x, 
                            providerName: "R", 
                            providerKey: adminRole.Name, 
                            tenantId: context.TenantId
                        ))
                        .ToList();
                    await _identityRoleRepository.InsertAsync(adminRole);
                    await _permissionGrantRepository.InsertManyAsync(adminRolePermissions);
                }

                // Dcp Manager
                if (currentRoles.All(c => c.Name != AppRole.DcpManager))
                {
                    var dcpManagerRole = new IdentityRole(Guid.NewGuid(), AppRole.DcpManager, context.TenantId)
                    {
                        IsStatic = false,
                        IsDefault = false,
                        IsPublic = true,
                    };
                    var dcpManagerRolePermissionNames = new List<string>
                    {
                        ReportsPermissions.DcpReportPermission,
                        ReportsPermissions.CreateNewDcpReport,
                        ReportsPermissions.GetDcpReportApprovalHistory,
                        ReportsPermissions.DcpReportApproval,
                        ReportsPermissions.GetMyDcpReport,
                        ReportsPermissions.RemoveDcpReport,
                        ReportsPermissions.GetDcpReportDetail,
                        ReportsPermissions.UpdateDcpReport,

                        ReportsPermissions.LRReportsPermission,
                        ReportsPermissions.CreateNewLRReport,
                        ReportsPermissions.GetLRApprovalHistory,
                        ReportsPermissions.LRReportApproval,
                        ReportsPermissions.GetMyLRReport,
                        ReportsPermissions.RemoveLRReport,
                        ReportsPermissions.GetLRReportDetail,
                        ReportsPermissions.UpdateLRReport,

                        TaskAssignmentPermissions.AssignDcpReport,
                        TaskAssignmentPermissions.AssignLessonRegisterReport,
                        TaskAssignmentPermissions.GetScheduleList,
                        TaskAssignmentPermissions.GetMyAssignedSchedule,

                        StatsPermissions.Rankings,
                        StatsPermissions.Statistics,

                        IdentityPermissions.Users.Default,
                        IdentityPermissions.Users.Create,
                        IdentityPermissions.Users.Update,
                        IdentityPermissions.Users.Delete,
                        IdentityPermissions.Users.ManagePermissions,
                    };
                    var dcpManagerRolePermissions = dcpManagerRolePermissionNames
                        .Select(x => new PermissionGrant(
                            id: Guid.NewGuid(),
                            name: x,
                            providerName: "R",
                            providerKey: dcpManagerRole.Name,
                            tenantId: context.TenantId
                        ))
                        .ToList();
                    await _identityRoleRepository.InsertAsync(dcpManagerRole);
                    await _permissionGrantRepository.InsertManyAsync(dcpManagerRolePermissions);
                }

                // Dcp ReporterStudent
                if (currentRoles.All(c => c.Name != AppRole.DcpReporterStudent))
                {
                    var dcpReporterRole = new IdentityRole(Guid.NewGuid(), AppRole.DcpReporterStudent, context.TenantId)
                    {
                        IsStatic = false,
                        IsDefault = false,
                        IsPublic = true,
                    };
                    var dcpReporterRolePermissionNames = new List<string>
                    {
                        ReportsPermissions.CreateNewDcpReport,
                        ReportsPermissions.GetDcpReportApprovalHistory,
                        ReportsPermissions.GetMyDcpReport,
                        ReportsPermissions.RemoveDcpReport,
                        ReportsPermissions.GetDcpReportDetail,
                        ReportsPermissions.UpdateDcpReport,
                    };
                    var dcpReporterRolePermissions = dcpReporterRolePermissionNames
                        .Select(x => new PermissionGrant(
                            id: Guid.NewGuid(),
                            name: x,
                            providerName: "R",
                            providerKey: dcpReporterRole.Name,
                            tenantId: context.TenantId
                        ))
                        .ToList();
                    await _identityRoleRepository.InsertAsync(dcpReporterRole);
                    await _permissionGrantRepository.InsertManyAsync(dcpReporterRolePermissions);
                }

                // LessonsRegisterReporter
                if (currentRoles.All(c => c.Name != AppRole.LessonsRegisterReporter))
                {
                    var lrReporterRole = new IdentityRole(Guid.NewGuid(), AppRole.LessonsRegisterReporter, context.TenantId)
                    {
                        IsStatic = false,
                        IsDefault = false,
                        IsPublic = true,
                    };
                    var lrReporterRolePermissionNames = new List<string>
                    {
                        ReportsPermissions.CreateNewLRReport,
                        ReportsPermissions.GetLRApprovalHistory,
                        ReportsPermissions.GetMyLRReport,
                        ReportsPermissions.RemoveLRReport,
                        ReportsPermissions.GetLRReportDetail,
                        ReportsPermissions.UpdateLRReport,
                    };
                    var lrReporterRolePermissions = lrReporterRolePermissionNames
                        .Select(x => new PermissionGrant(
                            id: Guid.NewGuid(),
                            name: x,
                            providerName: "R",
                            providerKey: lrReporterRole.Name,
                            tenantId: context.TenantId
                        ))
                        .ToList();
                    await _identityRoleRepository.InsertAsync(lrReporterRole);
                    await _permissionGrantRepository.InsertManyAsync(lrReporterRolePermissions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to seed roles and permissions", ex.Message);
            }

        }
    }
}
