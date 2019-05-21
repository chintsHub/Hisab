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

namespace Hisab.UI.Controllers
{
    [Authorize]
    public class AppHomeController : Controller
    {
        private SignInManager<ApplicationUser> _signInManager;
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;

        public AppHomeController(IEventManager eventManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _eventManager = eventManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var events = await _eventManager.GetEvents(user.Id);

            return View(new AppHomeVm(){userEvents = events});
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
            return View();
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
                var eventBo = new EventBO() { EventName = eventvm.NewEvent.EventName, UserId = user.Id };

                newEventId = await _eventManager.CreateEvent(eventBo);
               
            }

            return RedirectToAction("Dashboard", "Event", new {eventId = newEventId});
        }
    }
}