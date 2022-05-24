using Scool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scool.Users
{
    public interface IAccountManager
    {
        Task<bool> UserHasAccount(Guid userId);

        Task<bool> StudentHasAccount(Guid studentId);

        Task<bool> TeacherHasAccount(Guid teacherId);

        Task<Account> GetAccountByUserId(Guid userId);

        Task<Account> GetAccountByStudentId(Guid studentId);

        Task<Account> GetAccountByTeacherId(Guid teacherId);

        Task<Account> AddOrUpdateAccountForUser(Guid userId, Account profile);

        Task<Account> AddOrUpdateAccountForStudent(Guid studentId, Account profile);

        Task<Account> AddOrUpdateAccountForTeacher(Guid teacherId, Account profile);

        Task<bool> DeleteAccountForUser(Guid userId);

        Task<bool> DeleteAccountForStudent(Guid studentId);

        Task<bool> DeleteAccountForTeacher(Guid teacherId);
    }
}
