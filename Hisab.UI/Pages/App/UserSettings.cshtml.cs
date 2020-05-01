using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class UserSettingsModel : PageModel
    {
        [BindProperty(SupportsGet =true)]
        public UserSettingsVm UserSettingsVm { get; set; }


        public string UserSettingsMessage { get; set; }

        private IUserSettingManager _userSettingManager;
        private UserManager<ApplicationUser> _userManager;

        public UserSettingsModel(IUserSettingManager UserSettingManager, UserManager<ApplicationUser> userManager)
        {
            _userSettingManager = UserSettingManager;
            UserSettingsVm = new UserSettingsVm();
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGet()
        {
            UserSettingsBO user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userSettingManager.GetUserSettings(User.Identity.Name);
            }

            UserSettingsVm.UserName = user.NickName;
            UserSettingsVm.SelectedAvatarId = (int)user.Avatar;

            return Page();
        }

        public async Task<IActionResult> OnPostUserSettings()
        {
            
            if(ModelState.IsValid)
            {

                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var result = await _userSettingManager.UpdateUserSettings(UserSettingsVm.UserName, user.Id, (AvatarEnum)UserSettingsVm.SelectedAvatarId);

                if(result)
                {
                    UserSettingsMessage = "Settings updated sucessfully";
                }
                else
                {
                    ModelState.AddModelError("", "Error updating user settings");
                }

            }
            

            return Page();
        }
    }
}