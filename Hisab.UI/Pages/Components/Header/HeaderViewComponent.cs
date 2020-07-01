using Hisab.BL;
using Hisab.Common.BO;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Pages.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private IUserSettingManager _userSettingManager;
        private IConfiguration _configuration;

        

        public HeaderViewComponent(IUserSettingManager userSettingManager, IConfiguration configuration)
        {
            _userSettingManager = userSettingManager;
            _configuration = configuration;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new HeaderVm();
            var announcements = _configuration.GetSection("Announcements").Get<Announcements>();
            vm.HeaderAnnouncement = announcements.HeaderAnnouncement;

            UserSettingsBO user = null;
            if(User.Identity.IsAuthenticated)
            {
                user = await _userSettingManager.GetUserSettings(User.Identity.Name);
                vm.NickName = user.NickName;
                return View("default", vm);
            }
            

            return View("default", vm);
        }
    }
}
