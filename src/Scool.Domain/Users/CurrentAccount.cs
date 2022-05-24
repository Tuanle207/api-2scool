using Microsoft.AspNetCore.Http;
using Scool.AppConsts;
using Scool.Common;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Scool.Users
{
    public class CurrentAccount : ICurrentAccount, IScopedDependency
    {
        private readonly Account _currentAccount;
        private readonly ICurrentUser _currentUser;

        public CurrentAccount(ICurrentUser currentUser, IHttpContextAccessor httpContextAccessor)
        {
            _currentUser = currentUser;
            _currentAccount = httpContextAccessor.HttpContext?
                .Items[HttpContextConstants.CurrentAccountProperty] as Account;
        }

        protected Account Account => _currentAccount;
 
        public bool IsAuthenticated => _currentUser.IsAuthenticated;
        
        public bool HasAccount => Account != null;
        
        public bool IsStudent => StudentId.HasValue;
        
        public bool IsTeacher => TeacherId.HasValue;
        
        public Guid? Id => HasAccount ? Account.Id : null;
        
        public Guid? UserId => _currentUser.Id;
        
        public string DisplayName => HasAccount ? Account.DisplayName : null;
        
        public string Email => HasAccount ? Account.Email : null;
        
        public string PhoneNumber => HasAccount ? Account.PhoneNumber : null;
        
        public DateTime? Dob => HasAccount ? Account.Dob : null;
        
        public string Avatar => HasAccount ? Account.Avatar : null;
        
        public Guid? ClassId => HasAccount ? Account.ClassId : null;
        
        public Guid? StudentId => HasAccount ? Account.StudentId : null;
        
        public Guid? TeacherId => HasAccount ? Account.TeacherId : null;
        
        public DateTime? CreationTime => HasAccount ? Account.CreationTime : null;

        public Guid? CreatorId => HasAccount ? Account.CreatorId : null;
    }
}
