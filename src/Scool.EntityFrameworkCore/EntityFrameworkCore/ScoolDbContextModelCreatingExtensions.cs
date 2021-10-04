using Microsoft.EntityFrameworkCore;
using Scool.Domain.Common;
using Scool.Domain.Views;
using Scool.Users;
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

            /* Configure your own tables/entities inside here */

            builder.Entity<UserProfile>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(UserProfile), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Course>(b => 
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Course), ScoolConsts.DbSchema);

                // one Course has many Classes
                b.HasMany(b => b.Classes)
                    .WithOne(e => e.Course)
                    .HasForeignKey(f => f.CourseId);

                // one Course has many Activities
                b.HasMany(b => b.Activities)
                    .WithOne(e => e.Course)
                    .HasForeignKey(f => f.CourseId);
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

                // one Class has one form Teacher
                b.HasOne(b => b.FormTeacher)
                    .WithOne(e => e.FormClass)
                    .HasForeignKey<Class>(b => b.FormTeacherId);

                // one Class has one form Grade
                b.HasOne(b => b.Grade)
                    .WithMany()
                    .HasForeignKey(f => f.GradeId);

                // one Class has many students
                b.HasMany(b => b.Students)
                    .WithOne(e => e.Class)
                    .HasForeignKey(f => f.ClassId);

                b.HasMany<UserProfile>()
                    .WithOne(e => e.Class)
                    .HasForeignKey(f => f.ClassId);

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
                b.HasOne(b => b.Criteria)
                    .WithMany(b => b.Regulations)
                    .HasForeignKey(f => f.CriteriaId)
                    .IsRequired();
                b.ConfigureByConvention();
            });

            builder.Entity<Criteria>( b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Criteria), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<Activity>( b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(Activity), ScoolConsts.DbSchema);
                b.HasMany(b => b.Participants)
                    .WithOne(e => e.Activity)
                    .HasForeignKey(f => f.ActivityId);
                b.ConfigureByConvention();
            });

            builder.Entity<ActivityParticipant>( b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(ActivityParticipant), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<LessonsRegister>( b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(LessonsRegister), ScoolConsts.DbSchema);
                b.HasMany(b => b.AttachedPhotos)
                    .WithOne()
                    .HasForeignKey(f => f.LessonRegisterId);
                b.HasOne(b => b.Class)
                    .WithMany()
                    .HasForeignKey(f => f.ClassId);
                b.ConfigureByConvention();
            });

            builder.Entity<LessonRegisterPhotos>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(LessonRegisterPhotos), ScoolConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<TaskAssignment>( b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(TaskAssignment), ScoolConsts.DbSchema);
                b.HasOne(b => b.AssigneeProfile).WithMany().HasForeignKey(f => f.AssigneeId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpReport), ScoolConsts.DbSchema);
                b.HasMany(b => b.DcpClassReports)
                    .WithOne()
                    .HasForeignKey(f => f.DcpReportId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpClassReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpClassReport), ScoolConsts.DbSchema);
                b.HasMany(b => b.Faults)
                    .WithOne()
                    .HasForeignKey(f => f.DcpClassReportId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpClassReportItem>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpClassReportItem), ScoolConsts.DbSchema);
                b.HasOne(b => b.Regulation)
                    .WithMany()
                    .HasForeignKey(f => f.RegulationId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasMany(b => b.RelatedStudents)
                    .WithOne()
                    .HasForeignKey(f => f.DcpClassReportItemId);
                b.ConfigureByConvention();
            });

            builder.Entity<DcpStudentReport>(b =>
            {
                b.ToTable(ScoolConsts.DbTablePrefix + nameof(DcpStudentReport), ScoolConsts.DbSchema);
                b.HasOne(b => b.Student)
                    .WithMany()
                    .HasForeignKey(f => f.StudentId)
                    .OnDelete(DeleteBehavior.NoAction);
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