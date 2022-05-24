using System;

namespace Scool.Dtos
{
    public class CurrentAccountDto
    {
        public bool IsAuthenticated { get; set; }
        public bool HasAccount { get; set; }
        public bool IsStudent { get; set; }
        public bool IsTeacher { get; set; }
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Dob { get; set; }
        public string Avatar { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
    }
}
