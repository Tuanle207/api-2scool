using System;

namespace Scool.Users
{
    public interface ICurrentAccount
    {
        bool IsAuthenticated { get; }
        bool HasAccount { get; }
        bool IsStudent { get; }
        bool IsTeacher { get; }
        Guid? Id { get; }
        Guid? UserId { get; }
        string DisplayName { get; }
        string Email { get; }
        string PhoneNumber { get; }
        DateTime? Dob { get; }
        string Avatar { get; }
        Guid? ClassId { get; }
        Guid? StudentId { get; }
        Guid? TeacherId { get; }
        DateTime? CreationTime { get; }
        Guid? CreatorId { get; }
    }
}
