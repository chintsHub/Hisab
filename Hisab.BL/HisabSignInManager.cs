using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hisab.BL
{
    public class HisabSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
      

        public HisabSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider authenticationSchemeProvider)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, authenticationSchemeProvider)
        {
            _userManager = userManager;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByEmailAsync(userName);

            if(user != null)
            {
                var appUser = user as ApplicationUser;

                if (!appUser.IsUserActive)
                {
                    return await Task.FromResult<SignInResult>(SignInResult.LockedOut);
                }

                var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
                return result;
            }

            return new SignInResult();
        }

        
    }
}
