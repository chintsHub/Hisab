using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Hisab.UI.Controllers
{
    [Authorize]
    public class AppHomeController : Controller
    {
        private SignInManager<ApplicationUser> _signInManager;
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;
        private IEventInviteManager _eventInviteManager;

        public AppHomeController(IToastNotification toastNotification,IEventManager eventManager, IEventInviteManager eventInviteManager,
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _eventManager = eventManager;
            _userManager = userManager;
            _toastNotification = toastNotification;
            _eventInviteManager = eventInviteManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var events = await _eventManager.GetEvents(user.Id);

            var eventsVm = new List<UserEventVm>();

            foreach (var eventBo in events)
            {
                eventsVm.Add(new UserEventVm()
                {
                    EventId = eventBo.EventId,
                    EventName = eventBo.EventName,
                    CreatedUserNickName = eventBo.NickName
                });
            }

            return View(new AppHomeVm(){userEvents = eventsVm });
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
           
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Invites()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var invites = await _eventInviteManager.GetUserInvites(user.Id);

            var inviteVm = new List<EventInviteVm>();
            foreach (var eventvm in invites)
            {
                inviteVm.Add(new EventInviteVm()
                {
                    EventId = eventvm.EventId,
                    EventName = eventvm.EventName,
                    EventOwner = eventvm.EventOwner,
                    EventFriendId = eventvm.EventFriendId
                });
            }

            var retVal = new InviteVm() {Invites = inviteVm };

            return View(retVal);
        }

        [HttpPost]
        public async Task<IActionResult> JoinEvent(EventInviteVm inviteVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var rows = await _eventInviteManager.JoinEvent(inviteVm.EventFriendId, user.Id);
            _toastNotification.AddSuccessToastMessage("You have joined new Event. Congratulations!!");

            return RedirectToAction("Dashboard", "Event", new {eventId = inviteVm.EventId});
            
        }

        [HttpGet]
        public async Task<IActionResult> UserSettings()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(AppHomeVm eventvm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int newEventId = 0;

            if (user != null)
            {
                var eventBo = new NewEventBO()
                {
                    EventName = eventvm.NewEvent.EventName,
                    EventOwner = new EventFriendBO()
                    {
                        Email = user.Email,
                        NickName = user.NickName,
                        UserId = user.Id,
                        Status = EventFriendStatus.EventOwner

                    },
                    
                };

                newEventId = await _eventManager.CreateEvent(eventBo);
               
            }
            _toastNotification.AddSuccessToastMessage("Congratulations, you have created new Event");
            return RedirectToAction("Dashboard", "Event", new {eventId = newEventId});
        }
    }
}