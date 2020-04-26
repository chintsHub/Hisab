using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Hisab.UI
{
    public class EventsModel : PageModel
    {
        public List<EventCardVm> Events { get; set; }

        [BindProperty]
        public NewEventVm NewEvent { get; set; }

        
        

        public IActionResult OnGet()
        {
            Events = new List<EventCardVm>()
            {
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="~/img/eventCardImage1.jpg" , EventMessage="This event is created by"},
                new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="~/img/eventCardImage2.jpg" , EventMessage="This event is created by"},
                 new EventCardVm{ EventId = new Guid(), CreatedUserNickName="chints", EventName = "Thailand",
                    EventImagePath="~/img/eventCardImage1.jpg" , EventMessage="This event is created by"}
   

            };



            return Page();
        }

        public PartialViewResult OnGetEventModalLoad()
        {
            NewEvent = new NewEventVm();

            return new PartialViewResult
            {
                ViewName = "_EventModal",
                ViewData = new ViewDataDictionary<NewEventVm>(ViewData, NewEvent)
            };
        }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {

            }


            return new PartialViewResult
            {
                ViewName = "_EventModal",
                ViewData = new ViewDataDictionary<NewEventVm>(ViewData, NewEvent)
            };
        }
    }
}