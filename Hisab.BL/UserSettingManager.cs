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
        Task<bool> UpdateNickName(string nickName, int userId);
    }

    public class UserSettingManager : IUserSettingManager
    {
        
        private IDbConnectionProvider _connectionProvider;

        public UserSettingManager(IDbConnectionProvider connectionProvider)
        {
            
            _connectionProvider = connectionProvider;
        }
        public async Task<bool> UpdateNickName(string nickName, int userId)
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
