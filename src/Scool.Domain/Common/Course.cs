using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
namespace Scool.Common
{
    public class Course : Entity<Guid>, ISoftDelete, IMultiTenant
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public ICollection<Class> Classes { get; set; }
        public ICollection<Regulation> Regulations { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        public ICollection<TaskAssignment> TaskAssignments { get; set; }


        public Guid? TenantId { get; set; }
        public bool IsDeleted { get; set; }

        public Course()
        {
            IsActive = false;
            Classes = new List<Class>();
            Regulations = new List<Regulation>();
            Activities = new List<Activity>();
        }
    }

}