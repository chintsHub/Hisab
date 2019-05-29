using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using Microsoft.Extensions.Logging;
using NToastNotify;

namespace Hisab.UI.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;
        

        public EventController(IToastNotification toastNotification, IEventManager eventManager, UserManager<ApplicationUser> userManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _toastNotification = toastNotification;
            
        }

        [HttpGet]
        [Route("/Event/Dashboard/{eventId}")]
        public async Task<IActionResult> Dashboard(int eventId)
        {

            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);

            _toastNotification.AddInfoToastMessage("Event loaded!");
            _toastNotification.AddInfoToastMessage("Event loaded 2!");

            return View("Index", new EventVm() {EventId = eventBo.EventId, EventName = eventBo.EventName});

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
                    Status = friend.Status,
                    EventFriendId = friend.EventFriendId
                });
            }

            _toastNotification.AddInfoToastMessage("Friends loaded!");

            return View("Friends", new EventVm() { EventId = eventBo.EventId, EventName = eventBo.EventName, Friends = friendList });
        }

        public async Task<IActionResult> DisableFriend(EventFriendVm eventFriendVm)
        {
            var result = await _eventManager.DisableFriend(eventFriendVm.EventFriendId);

            if(result)
                _toastNotification.AddSuccessToastMessage("Friend made InActive");

            return RedirectToAction("Friends", new {eventFriendVm.EventId});
        }

        public async Task<IActionResult> UpdateFriend(EventFriendVm eventFriendVm)
        {
            return null;
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
        public async Task<IActionResult> Settings(int eventId)
        {
            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);

            var eventVm = new EventVm() {EventId = eventBo.EventId, EventName = eventBo.EventName, Friends = null};
            eventVm.StatusList.FirstOrDefault(x => x.Text == eventBo.Status.GetDescription()).Selected = true;

            return View("Settings", eventVm);
        }

        public async Task<IActionResult> UpdateEvent(EventVm eventVm)
        {
            if (ModelState.IsValid)
            {
                EventStatus status;
                Enum.TryParse(eventVm.SelectedStatus,out status);
                var result = await _eventManager.UpdateEvent(eventVm.EventName, eventVm.EventId, status);

                if (result)
                {
                    _toastNotification.AddSuccessToastMessage("Event name updated sucessfully.");
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Couldn't update event name");
                    return View("Settings", eventVm);
                }
                   

            }
            else
            {
                _toastNotification.AddErrorToastMessage("Invalid data passed");
                return View("Settings", eventVm);
            }

            return RedirectToAction("Settings", new {eventVm.EventId});
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(EventFriendVm newEventFriend)
        {
            _toastNotification.RemoveAll();
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var newFriend = await _eventManager.CreateEventFriend(new EventFriendBO()
                    {
                        Email = newEventFriend.Email,
                        AdultCount = newEventFriend.AdultCount,
                        EventId = newEventFriend.EventId,
                        KidsCount = newEventFriend.KidsCount,
                        NickName = newEventFriend.Name

                    });

                    if (newFriend)
                        _toastNotification.AddSuccessToastMessage($"Sucessfully added Friend {newEventFriend.Name} to this event.");


                }
                catch (Exception ex)
                {
                    if (ex is HisabException)
                    {
                        _toastNotification.AddErrorToastMessage("Error:" + ex.Message);

                    }
                    else
                    {
                        throw;
                    }
                }

            }
            else
            {
                _toastNotification.AddErrorToastMessage("Invalid data passed");
            }

            
            
            
            return RedirectToAction("Friends", new { newEventFriend.EventId});
        }
    }

    
}