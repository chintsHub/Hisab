using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
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

        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;



        public EventsModel(IEventManager eventManager, UserManager<ApplicationUser> userManager )
        {
            _eventManager = eventManager;
            _userManager = userManager;
        }
        

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

        //public PartialViewResult OnGetEventModalLoad()
        //{
        //    NewEvent = new NewEventVm();

        //    return new PartialViewResult
        //    {
        //        ViewName = "_EventModal",
        //        ViewData = new ViewDataDictionary<NewEventVm>(ViewData, NewEvent)
        //    };
        //}

        public async Task<IActionResult> OnPostCreateEvent()
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                

                if (user != null)
                {
                    var eventBo = new NewEventBO()
                    {
                        EventName = NewEvent.EventName,
                        EventOwner = new NewEventFriendBO()
                        {
                            UserId = user.Id,
                            Status = EventFriendStatus.Owner

                        },
                        EventPic = HisabImageManager.GetRandomEventImage()
                    };

                    var eventId = await _eventManager.CreateEvent(eventBo);
                    NewEvent.Url = Url.Page("/App/Events/Dashboard", new { id = eventId });
                    
                    return new RedirectResult(NewEvent.Url);
                }
            }

            var jsonObject = new { errorMessage = "Invalid data. Please provide Event Name." };

            return new JsonResult(jsonObject);
            
        }
    }
}