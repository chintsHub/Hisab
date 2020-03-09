using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Pages.Components.EventCard
{
    public class EventCardViewComponent : ViewComponent
    {
        public EventCardViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(EventCardVm eventCardVm)
        {
            var events = new List<EventCardVm>()
            {
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand"}

            };

            

            return View(eventCardVm);
        }
    }
}
