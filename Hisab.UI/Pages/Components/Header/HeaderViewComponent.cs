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

        public string Announcement { get; set; }

        public HeaderViewComponent(IUserSettingManager userSettingManager, IConfiguration configuration)
        {
            _userSettingManager = userSettingManager;
            _configuration = configuration;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            UserSettingsBO user = null;
            if(User.Identity.IsAuthenticated)
            {
                user = await _userSettingManager.GetUserSettings(User.Identity.Name);
                return View("default", user.NickName);
            }
            var announcements = _configuration.GetSection("Announcements").Get<Announcements>();

            return View("default", announcements.HeaderAnnouncement);
        }
    }
}
