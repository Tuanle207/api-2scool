using Microsoft.AspNetCore.Http;
using Scool.AppConsts;
using System;

namespace Scool.Courses
{
    public interface IActiveCourse
    {
        public bool IsAvailable { get; }
     
        public Guid? Id { get; }

        public string Name { get; }

        public string Description { get; }

        public DateTime? StartTime { get; }

        public DateTime? FinishTime { get; }

        public bool? Ended { get; }
    }
}
