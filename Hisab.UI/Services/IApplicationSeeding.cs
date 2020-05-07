using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common;
using Hisab.Common.Log;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

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
                NickName = "admin",
                IsUserActive = true

            };

           
            
            var result = await _userManager.CreateAsync(user, "01hisabS!");
            
            if (result.Succeeded)
            {
                Log.Write(LogEventLevel.Information, "{@LogDetail}", LogHelper.CreateLogDetail(LogType.Diagnostic, "Sucessfully created admin@hisab.io user", LogLayer.Server));


                var r = await _userManager.AddToRoleAsync(user, "Admin");
                if (r.Succeeded)
                {
                    Log.Write(LogEventLevel.Information, "{@LogDetail2}", LogHelper.CreateLogDetail(LogType.Diagnostic, "Sucessfully added user to Admin role", LogLayer.Server));
                }
            }
           
        }
    }
}
