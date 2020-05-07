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
    }

    public class UserSettingManager : IUserSettingManager
    {
        
        private IDbConnectionProvider _connectionProvider;

        public UserSettingManager(IDbConnectionProvider connectionProvider)
        {
            
            _connectionProvider = connectionProvider;
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
