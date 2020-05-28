using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hisab.Common.BO;
using Hisab.Dapper;
using Hisab.Dapper.Repository;

namespace Hisab.BL
{
    public interface IUserSettingManager
    {
        Task<bool> UpdateUserSettings(string nickName, Guid userId, AvatarEnum avatar);

        Task<bool> UpdateUserSettings(string nickName, Guid userId, bool isUserActive, bool emailCofirmed);

        Task<UserSettingsBO> GetUserSettings(string userName);

        Task<bool> CreateUserAccounts(Guid userId);
    }

    public class UserSettingManager : IUserSettingManager
    {
        
        private IDbConnectionProvider _connectionProvider;

        public UserSettingManager(IDbConnectionProvider connectionProvider)
        {
            
            _connectionProvider = connectionProvider;
        }

        public async Task<bool> CreateUserAccounts(Guid userId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var cashAccount = new UserAccountBO() { UserId = userId, AccountId = System.Guid.NewGuid(), AccountType = ApplicationAccountType.Cash };
                var expenseAccount = new UserAccountBO() { UserId = userId, AccountId = System.Guid.NewGuid(), AccountType = ApplicationAccountType.Expense };
                var accountReceive = new UserAccountBO() { UserId = userId, AccountId = System.Guid.NewGuid(), AccountType = ApplicationAccountType.AccountRecievable };
                var accountPayable = new UserAccountBO() { UserId = userId, AccountId = System.Guid.NewGuid(), AccountType = ApplicationAccountType.AccountPayable };

                var accounts = new List<UserAccountBO>();
                accounts.Add(cashAccount);
                accounts.Add(expenseAccount);
                accounts.Add(accountReceive);
                accounts.Add(accountPayable);

                var result = context.ApplicationUserRepository.CreateUserAccounts(accounts);

                context.SaveChanges();

                return result;

            }
        }

        public async Task<UserSettingsBO> GetUserSettings(string userName)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var user = await context.ApplicationUserRepository.FindByNameAsync(userName);

                return new UserSettingsBO 
                { 
                    NickName = user.NickName, 
                    Avatar = (AvatarEnum) user.AvatarId ,
                    IsUserActive = user.IsUserActive,
                    EmailConfirmed = user.EmailConfirmed
                };
                

            }
        }

        public async Task<bool> UpdateUserSettings(string nickName, Guid userId, AvatarEnum avatar)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.ApplicationUserRepository.UpdateUserSettings(nickName, userId,(int)avatar);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> UpdateUserSettings(string nickName, Guid userId, bool isUserActive, bool emailCofirmed)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.ApplicationUserRepository.UpdateUserSettings(nickName, userId, isUserActive, emailCofirmed);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }
    }
}
