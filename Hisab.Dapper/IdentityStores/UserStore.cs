using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hisab.Dapper.IdentityStores
{
    public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
    {
        
        private IDbConnectionProvider dbConnectionProvider;

        public UserStore(IDbConnectionProvider dbConnectionProvider)
        {
            
            this.dbConnectionProvider = dbConnectionProvider;

        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(dbConnectionProvider))
            {
                //await context.InitializeWithTransaction();
                user.Id = System.Guid.NewGuid();

                var result =  await context.ApplicationUserRepository.CreateAsync(user, cancellationToken);

                if (result.Succeeded)
                {
                    context.SaveChanges();
                }

                return result;
            }

        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            int i = 10;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {

            using (var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider))
            {
                //await context.InitializeWithTransaction();

                return  await context.ApplicationUserRepository.FindByIdAsync(userId);

            }
            
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            //using (var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider))
            //{
            //await context.InitializeWithTransaction();
            var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider);
                var user =  await context.ApplicationUserRepository.FindByNameAsync(normalizedUserName);

                return user;

            //}
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(dbConnectionProvider))
            {
                //await context.InitializeWithTransaction();

                var result = await context.ApplicationUserRepository.UpdateAsync(user, cancellationToken);

                if (result.Succeeded)
                {
                    context.SaveChanges();
                }

                return result;
            }
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(dbConnectionProvider))
            {
                
                var role = await context.ApplicationRoleRepository.FindByRoleNameAsync(roleName);


                var result =  await context.ApplicationUserRepository.AddUserToRole(user.Id, role.Id);

                context.SaveChanges();
                 
            }
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            
            using (var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider))
            {
                
                return await context.ApplicationUserRepository.GetRolesAsync(user.Id);
            }
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            using (var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider))
            {
                
                var role = await context.ApplicationRoleRepository.FindByRoleNameAsync(roleName);

                if (role == null)
                {
                    return false;
                }

                return await context.ApplicationUserRepository.IsUserInRole(user.Id, role.Id);


            }
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using (var context = await HisabContextFactory.InitializeAsync(dbConnectionProvider))
            {
                //await context.InitializeWithTransaction();

                var user = await context.ApplicationUserRepository.FindByEmailAsync(normalizedEmail);

                return user;

            }
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }
    }
}
