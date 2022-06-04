using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Courses;
using Scool.Users;
using Scool.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Scool.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See ScoolMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class ScoolDbContext : AbpDbContext<ScoolDbContext>
    {
        protected ICurrentAccount CurrentAccount => LazyServiceProvider.LazyGetRequiredService<ICurrentAccount>();
        protected IActiveCourse ActiveCourse => LazyServiceProvider.LazyGetRequiredService<IActiveCourse>();

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Activity> Activitys { get; set; }
        public DbSet<ActivityParticipant> ActivityParticipants { get; set; }
        public DbSet<Criteria> Criterias { get; set; }
        public DbSet<Regulation> Regulations { get; set; }
        public DbSet<LessonsRegister> LessonsRegisters { get; set; }
        public DbSet<LessonRegisterPhotos> LessonRegisterPhotos { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<DcpReport> DcpReports { get; set; }
        public DbSet<DcpClassReport> DcpClassReports { get; set; }
        public DbSet<DcpClassReportItem> DcpClassReportItems { get; set; }
        public DbSet<DcpStudentReport> DcpStudentReports { get; set; }
        public DbSet<AppNotification> AppNotifications { get; set; }
        public DbSet<UserNotificationCount> UserNotificationCounts { get; set; }



        // not actually stored in DB
        public DbSet<OverallClassRanking> OverallClassRanking { get; set; }
        public DbSet<DcpClassRanking> DcpClassRankings { get; set; }
        public DbSet<LrClassRanking> LrClassRankings { get; set; }
        public DbSet<DcpClassFault> DcpClassFaults { get; set; }
        public DbSet<CommonDcpFault> CommonDcpFaults { get; set; }
        public DbSet<StudentWithMostFaults> StudentWithMostFaults { get; set; }



        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside ScoolDbContextModelCreatingExtensions.ConfigureScool
         */

        public ScoolDbContext(DbContextOptions<ScoolDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Configure the shared tables (with included modules) here */

            builder.Entity<AppUser>(b =>
            {
                b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser
                
                b.ConfigureByConvention();
                b.ConfigureAbpUser();

                /* Configure mappings for your additional properties
                 * Also see the ScoolEfCoreEntityExtensionMappings class
                 */

            });

            /* Configure your own tables/entities inside the ConfigureScool method */

            builder.ConfigureScool();
        }

        private void Presave()
        {
            var currentAccountId = CurrentAccount.Id;
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {

                    if (entry.Entity is IHaveCreationInfo)
                    {
                        var entity = entry.Entity as IHaveCreationInfo;
                        entity.CreatorId = currentAccountId;
                        entity.CreationTime = DateTime.UtcNow;
                    }
                    if (entry.Entity is IHaveCourse && ActiveCourse.IsAvailable)
                    {
                        var entity = entry.Entity as IHaveCourse;
                        entity.CourseId = ActiveCourse.Id.Value;
                    }
                }
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IHaveUpdationInfo )
                    {
                        var entity = entry.Entity as IHaveUpdationInfo;
                        entity.LastUpdatorId = currentAccountId;
                        entity.LastUpdationTime = DateTime.UtcNow;
                    }
                }
            }
    }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            Presave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Presave();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            Presave();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Presave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }
}
