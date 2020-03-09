using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class EventsModel : PageModel
    {
        public List<EventCardVm> Events { get; set; }

        public IActionResult OnGet()
        {
            Events = new List<EventCardVm>()
            {
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="/img/eventCardImage1.jpg" , EventMessage="This event is created by"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="/img/eventCardImage2.jpg" , EventMessage="This event is created by"},
                 new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="/img/eventCardImage1.jpg" , EventMessage="This event is created by"}
   

            };



            return Page();
        }
    }
}