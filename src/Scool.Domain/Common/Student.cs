using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Student : Entity<Guid>
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public Class Class { get; set; }
        public DateTime Dob { get; set; }
        public string ParentPhoneNumber { get; set; }
    }
}