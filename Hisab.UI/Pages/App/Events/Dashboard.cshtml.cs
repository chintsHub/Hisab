using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class DashboardModel : PageModel
    {
        private IEventManager _eventManager;

        [BindProperty(SupportsGet =true)]
        public EventVm Event { get; set; }

        public DashboardModel(IEventManager eventManager)
        {
            _eventManager = eventManager;
            
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



            return Page();
        }
    }
}