using Hisab.Dapper.Identity;
using System;
using System.Collections.Generic;
using System.Data;
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
    }

    internal class ApplicationUserRepository : RepositoryBase, IApplicationUserRepository
    {
        public ApplicationUserRepository(IDbTransaction transaction) : base(transaction)
        {

        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string command = $@"

            INSERT INTO [ApplicationUser] ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled])
                    VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                    @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.EmailConfirmed)}, @{nameof(ApplicationUser.PasswordHash)},
                    @{nameof(ApplicationUser.PhoneNumber)}, @{nameof(ApplicationUser.PhoneNumberConfirmed)}, @{nameof(ApplicationUser.TwoFactorEnabled)});
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            user.Id = await Connection.QuerySingleAsync<int>(command, user, transaction: Transaction);

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
                                   "WHERE Id = @Id;";

           return await Connection.QuerySingleOrDefaultAsync<ApplicationUser>(command, transaction: Transaction);
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
    }
}
