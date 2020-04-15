using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hisab.Dapper;
using Hisab.Dapper.Repository;

namespace Hisab.BL
{
    public interface IUserSettingManager
    {
        Task<bool> UpdateNickName(string nickName, Guid userId);

        Task<string> GetNickName(string userName);
    }

    public class UserSettingManager : IUserSettingManager
    {
        
        private IDbConnectionProvider _connectionProvider;

        public UserSettingManager(IDbConnectionProvider connectionProvider)
        {
            
            _connectionProvider = connectionProvider;
        }

        public async Task<string> GetNickName(string userName)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var user = await context.ApplicationUserRepository.FindByNameAsync(userName);

                return user.NickName;
                

            }
        }

        public async Task<bool> UpdateNickName(string nickName, Guid userId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.ApplicationUserRepository.UpdateNickName(nickName, userId);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }
    }
}
