﻿using Hisab.Dapper.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Hisab.Dapper.Repository
{
    public interface  IApplicationUserRepository
    {
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<ApplicationUser> FindByNameAsync(string normalizedUserName);
        Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken);

        Task<IdentityResult> AddUserToRole(int userId, int roleId);
        Task<bool> IsUserInRole(int userId, int roleId);
        Task<IList<string>> GetRolesAsync(int userId);
    }

    internal class ApplicationUserRepository : RepositoryBase, IApplicationUserRepository
    {
        public ApplicationUserRepository(IDbTransaction transaction) : base(transaction)
        {

        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string command = $@"

            INSERT INTO [ApplicationUser] ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [NickName])
                    VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                    @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.EmailConfirmed)}, @{nameof(ApplicationUser.PasswordHash)},
                    @{nameof(ApplicationUser.PhoneNumber)}, @{nameof(ApplicationUser.PhoneNumberConfirmed)}, @{nameof(ApplicationUser.TwoFactorEnabled)}, @{nameof(ApplicationUser.NickName)});
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            user.Id = await Connection.QuerySingleAsync<int>(command, user, transaction: Transaction);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            await Connection.ExecuteAsync($@"UPDATE [ApplicationUser] SET
                    [UserName] = @{nameof(ApplicationUser.UserName)},
                    [NormalizedUserName] = @{nameof(ApplicationUser.NormalizedUserName)},
                    [Email] = @{nameof(ApplicationUser.Email)},
                    [NormalizedEmail] = @{nameof(ApplicationUser.NormalizedEmail)},
                    [EmailConfirmed] = @{nameof(ApplicationUser.EmailConfirmed)},
                    [PasswordHash] = @{nameof(ApplicationUser.PasswordHash)},
                    [PhoneNumber] = @{nameof(ApplicationUser.PhoneNumber)},
                    [PhoneNumberConfirmed] = @{nameof(ApplicationUser.PhoneNumberConfirmed)},
                    [TwoFactorEnabled] = @{nameof(ApplicationUser.TwoFactorEnabled)},
                    [NickName] = @{nameof(ApplicationUser.NickName)}
                    WHERE [Id] = @{nameof(ApplicationUser.Id)}", user,Transaction);

             return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddUserToRole(int userId, int roleId)
        {
            string command = $@"INSERT INTO [ApplicationUserRole] ([UserId], [RoleId])
                    VALUES (@{nameof(userId)}, @{nameof(roleId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            var rows = await Connection.ExecuteAsync(command, new{ userId , roleId }, transaction: Transaction);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            const string command = "SELECT * " +
                                   "FROM dbo.Users " +
                                   "WHERE Id = @userId;";

            return await Connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                    WHERE [Id] = @{nameof(userId)}", new { userId },Transaction);
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            string command = $@"SELECT * FROM [ApplicationUser] WHERE [NormalizedUserName] = @{nameof(normalizedUserName)}";

            return await Connection.QuerySingleOrDefaultAsync<ApplicationUser>(command, transaction: Transaction,param: new { normalizedUserName });
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserInRole(int userId, int roleId)
        {
            var result =  await Connection.ExecuteScalarAsync<int>($@"SELECT count(*) FROM [ApplicationUserRole]
                    WHERE [UserId] = @{nameof(userId)} and [RoleId] = @{nameof(roleId)}", new { userId, roleId }, Transaction);

            return result > 0;
        }

        public async Task<IList<string>> GetRolesAsync(int userId)
        {
            var queryResults = await Connection.QueryAsync<string>("SELECT r.[Name] FROM [ApplicationRole] r INNER JOIN [ApplicationUserRole] ur ON ur.[RoleId] = r.Id " +
                                                                   "WHERE ur.UserId = @userId", new { userId },Transaction);

            return queryResults.ToList();
        }
    }
}
