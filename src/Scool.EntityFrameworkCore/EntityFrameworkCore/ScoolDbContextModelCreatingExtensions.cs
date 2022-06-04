using Microsoft.EntityFrameworkCore;
using Scool.Common;
using Scool.Users;
using Scool.Views;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Scool.EntityFrameworkCore
{
    public static class ScoolDbContextModelCreatingExtensions
    {
        public static void ConfigureScool(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));


            /* Config "VIEW" DbSet just for carry querying data */
            builder.Entity<DcpClassRanking>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(DcpClassRanking), ScoolConsts.DbSchema);
            });
            builder.Entity<LrClassRanking>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(LrClassRanking), ScoolConsts.DbSchema);
            });
            builder.Entity<DcpClassFault>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(DcpClassFault), ScoolConsts.DbSchema);
            });
            builder.Entity<CommonDcpFault>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(CommonDcpFault), ScoolConsts.DbSchema);
            });
            builder.Entity<StudentWithMostFaults>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(StudentWithMostFaults), ScoolConsts.DbSchema);
            });
            builder.Entity<OverallClassRanking>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(OverallClassRanking), ScoolConsts.DbSchema);
            });
            builder.Entity<ClassFaultDetail>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(ClassFaultDetail), ScoolConsts.DbSchema);
            });
            builder.Entity<FaultDetail>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(FaultDetail), ScoolConsts.DbSchema);
            });
            builder.Entity<StudentFaultDetail>(b =>
            {
                b.HasNoKey();
                b.ToView(nameof(StudentFaultDetail), ScoolConsts.DbSchema);
            });
            //builder.Entity<LrClassRanking>(b =>
            //{
            //    b.HasNoKey();
            //    b.ToView(nameof(OverallClassRanking), ScoolConsts.DbSchema);
            //});
            /* Configure your own tables/entities inside here */

            builder.Entity<Account>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Account), ScoolConsts.DbSchema);
                b.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId);
                b.HasOne(x => x.Student).WithOne().HasForeignKey<Account>(x => x.StudentId);
                b.HasOne(x => x.Teacher).WithOne().HasForeignKey<Account>(x => x.TeacherId);
                b.ConfigureByConvention();
            });

            builder.Entity<Course>(b => 
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Course), ScoolConsts.DbSchema);
                b.HasMany(b => b.Classes).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction); ;
                b.HasMany(b => b.Regulations).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction); ;
                b.HasMany(b => b.Activities).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction); ;
                b.HasMany(b => b.Students).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction); ;
                b.HasMany(b => b.Teachers).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction); ;
                b.HasMany(b => b.TaskAssignments).WithOne(e => e.Course).HasForeignKey(f => f.CourseId).OnDelete(DeleteBehavior.NoAction);
                b.ConfigureByConvention();
            });
            
            builder.Entity<Grade>(b => 
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Grade), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Class>(b => 
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Class), ScoolConsts.DbSchema);
                b.HasOne(b => b.FormTeacher).WithOne(e => e.FormClass).HasForeignKey<Class>(b => b.FormTeacherId);
                b.HasOne(b => b.Grade).WithMany().HasForeignKey(f => f.GradeId);
                b.HasMany(b => b.Students).WithOne(e => e.Class).HasForeignKey(f => f.ClassId);
                b.ConfigureByConvention();
            });

            builder.Entity<Student>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Student), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Teacher>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Teacher), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Regulation>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Regulation), ScoolConsts.DbSchema);
                b.HasOne(b => b.Criteria).WithMany(b => b.Regulations).HasForeignKey(f => f.CriteriaId);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<Criteria>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Criteria), ScoolConsts.DbSchema);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<Activity>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Activity), ScoolConsts.DbSchema);
                b.HasMany(b => b.Participants).WithOne(e => e.Activity).HasForeignKey(f => f.ActivityId);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.HasOne(x => x.LastUpdatorAccount).WithMany().HasForeignKey(x => x.LastUpdatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<ActivityParticipant>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(ActivityParticipant), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<LessonsRegister>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(LessonsRegister), ScoolConsts.DbSchema);
                b.HasMany(b => b.AttachedPhotos).WithOne().HasForeignKey(f => f.LessonRegisterId);
                b.HasOne(b => b.Class).WithMany().HasForeignKey(f => f.ClassId);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<LessonRegisterPhotos>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(LessonRegisterPhotos), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<TaskAssignment>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(TaskAssignment), ScoolConsts.DbSchema);
                b.HasOne(b => b.Assignee).WithMany().HasForeignKey(f => f.AssigneeId);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpReport), ScoolConsts.DbSchema);
                b.HasMany(b => b.DcpClassReports).WithOne(x => x.DcpReport).HasForeignKey(f => f.DcpReportId);
                b.HasOne(x => x.CreatorAccount).WithMany().HasForeignKey(x => x.CreatorId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpClassReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpClassReport), ScoolConsts.DbSchema);
                b.HasMany(b => b.Faults).WithOne(x => x.DcpClassReport).HasForeignKey(f => f.DcpClassReportId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpClassReportItem>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpClassReportItem), ScoolConsts.DbSchema);
                b.HasOne(b => b.Regulation).WithMany().HasForeignKey(f => f.RegulationId).OnDelete(DeleteBehavior.NoAction);
                b.HasMany(b => b.RelatedStudents).WithOne(x => x.DcpClassReportItem).HasForeignKey(f => f.DcpClassReportItemId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpStudentReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpStudentReport), ScoolConsts.DbSchema);
                b.HasOne(b => b.Student).WithMany().HasForeignKey(f => f.StudentId).OnDelete(DeleteBehavior.NoAction);
                b.ConfigureByConvention();
            });

            builder.Entity<AppNotification>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(AppNotification), ScoolConsts.DbSchema);
                b.HasOne(x => x.FromAccount).WithMany().HasForeignKey(f => f.FromAccountId);
                b.ConfigureByConvention();
            });

            builder.Entity<UserNotificationCount>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(UserNotificationCount), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            // builder.Entity<YourEntityHere>( b =>
            // {
            //     b.ToTable(ScoolConsts.DbTablePrefix + nameof(YourEntityHere), ScoolConsts.DbSchema);
            //     
            //     b.ConfigureByConvention();
            // });
        }
    }
}