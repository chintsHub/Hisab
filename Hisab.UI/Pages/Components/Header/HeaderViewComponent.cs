using Hisab.BL;
using Hisab.Common.BO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Pages.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private IUserSettingManager _userSettingManager;

        public HeaderViewComponent(IUserSettingManager userSettingManager)
        {
            _userSettingManager = userSettingManager;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            UserSettingsBO user = null;
            if(User.Identity.IsAuthenticated)
            {
                user = await _userSettingManager.GetUserSettings(User.Identity.Name);
                return View("default", user.NickName);
            }
            
            return View("default","");
        }
    }
}
