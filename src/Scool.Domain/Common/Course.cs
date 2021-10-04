using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Course : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public ICollection<Class> Classes { get; set; }
        public ICollection<Regulation> Regulations { get; set; }
        public ICollection<Activity> Activities { get; set; }

        public Course()
        {
            Classes = new List<Class>();
            Regulations = new List<Regulation>();
            Activities = new List<Activity>();
        }
    }

}