using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Dapper.Identity;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class DashboardModel : PageModel
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;

        [BindProperty(SupportsGet =true)]
        public EventVm Event { get; set; }

        
        public bool IsLoggedInUserTheEventAdmin { get; set; }

        public DashboardModel(IEventManager eventManager, UserManager<ApplicationUser> userManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;


        }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            
            var eve = await _eventManager.GetEventById(Id);

            
            Event = new EventVm()
            {
                EventId = eve.Id,
                EventName = eve.EventName,

            };

            foreach (var f in eve.Friends)
            {
                Event.Friends.Add(new EventFriendVm()
                {
                    EventId = f.EventId,
                    UserId = f.UserId,
                    Email = f.Email,
                    Name = f.NickName,
                    Status = f.EventFriendStatus.GetDescription(),
                    Avatar = HisabImageManager.GetAvatar(f.Avatar)
                });
            }

            var eventAdmin = eve.Friends.Where(x => x.EventFriendStatus == Common.BO.EventFriendStatus.EventAdmin).First().Email;

            if (eventAdmin.ToLower() == User.Identity.Name.ToLower())
                IsLoggedInUserTheEventAdmin = true;

            return Page();
        }
    }
}