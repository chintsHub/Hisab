using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Hisab.UI
{
    [Authorize(Roles ="App User, Admin")]
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
        

        public async Task<IActionResult> OnGet()
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var events = await _eventManager.GetEvents(user.Id);

            Events = new List<EventCardVm>();

            foreach (var eventBo in events.Where(e => e.EventStatus == EventStatus.Active))
            {
                Events.Add(new EventCardVm()
                {
                    EventId = eventBo.Id,
                    EventName = eventBo.EventName,
                    CreatedUserNickName = eventBo.OwnerName,
                    EventMessage = "Event Created by: " + eventBo.OwnerName,
                    EventImagePath = HisabImageManager.GetEventImages()
                                    .Where<HisabImage>(x => x.Id == eventBo.EventPic).FirstOrDefault().ImagePath
                });
            }
            

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
                            Status = EventFriendStatus.EventAdmin

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