using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Extensions;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hisab.UI.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;

        public EventController(IEventManager eventManager, UserManager<ApplicationUser> userManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("/Event/Dashboard/{eventId}")]
        public async Task<IActionResult> Dashboard(int eventId)
        {

            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);
            

            return View("Index",new EventVm(){EventId = eventBo.EventId, EventName = eventBo.EventName})
                .WithSuccess("Welcome mate","This is your event");
        }

        [HttpGet]
        [Route("Event/Friends/{eventId}")]
        public async Task<IActionResult> Friends(int eventId)
        {
            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);
            var friendList = new List<EventFriendVm>();
            foreach (var friend in eventBo.Friends)
            {
                friendList.Add(new EventFriendVm()
                {
                    Name = friend.NickName,
                    Email = friend.Email,
                    AdultCount = friend.AdultCount,
                    EventId = eventId,
                    KidsCount = friend.KidsCount,
                    Status = friend.Status
                });
            }

            return View("Friends", new EventVm() { EventId = eventBo.EventId, EventName = eventBo.EventName, Friends = friendList });
        }

        [HttpGet]
        [Route("Event/Transactions/{eventId}")]
        public IActionResult Transactions(int eventId)
        {
            return View();
        }

        [HttpGet]
        [Route("Event/Settlement/{eventId}")]
        public IActionResult Settlement(int eventId)
        {
            return View();
        }

        [HttpGet]
        [Route("Event/Settings/{eventId}")]
        public IActionResult Settings(int eventId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(EventFriendVm eventVm)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var newFriend =  await _eventManager.CreateEventFriend(new EventFriendBO()
                    {
                        Email = eventVm.Email,
                        AdultCount = eventVm.AdultCount,
                        EventId = eventVm.EventId,
                        KidsCount = eventVm.KidsCount,
                        NickName = eventVm.Name

                    });

                    eventVm.SuccessMessage = "Sucessfully added Friend to this event.";
                    return RedirectToAction("Friends", new { eventVm.EventId });
                }
                catch (Exception ex)
                {
                    if (ex is HisabException)
                    {
                        ModelState.AddModelError(string.Empty,ex.Message);
                        return View("Friends", eventVm);
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }

            ModelState.AddModelError(string.Empty, "Invalid data passed");
            return RedirectToAction("Friends", new { eventVm.EventId});
        }
    }

    
}