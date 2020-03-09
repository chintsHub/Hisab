using Hisab.BL;
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
            string nickName = "";
            if(User.Identity.IsAuthenticated)
            {
                nickName = await _userSettingManager.GetNickName(User.Identity.Name);
            }
            
            return View("default",nickName);
        }
    }
}
