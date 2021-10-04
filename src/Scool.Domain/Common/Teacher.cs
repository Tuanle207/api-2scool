using System;
using Volo.Abp.Domain.Entities;

namespace Scool.Domain.Common
{
    public class Teacher : Entity<Guid>
    {
        public string Name { get; set; }
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Class FormClass { get; set; }
    }
}