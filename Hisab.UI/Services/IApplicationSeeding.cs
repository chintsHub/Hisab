using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hisab.UI.Services
{
    public interface IApplicationSeeding
    {
        Task CreateAdminUser();
    }

    public class ApplicationSeeding : IApplicationSeeding
    {
        private UserManager<ApplicationUser> _userManager;
       

        public ApplicationSeeding(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task CreateAdminUser()
        {
            var user = new ApplicationUser
            {
                Email = "admin@hisab.io",
                EmailConfirmed = true,
                UserName = "admin@hisab.io",
                NickName = "admin"

            };
            
            var result = await _userManager.CreateAsync(user, "01hisabS!");
            
            if (result.Succeeded)
            {
               // log user created
               var r = await _userManager.AddToRoleAsync(user, "Admin");
                if (r.Succeeded)
                {
                    //log user added to role
                }
            }
            
        }
    }
}
