using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Dashboard(int eventId)
        {
            
            return View("Index");
        }

        [HttpGet]
        [Route("Event/Friends/{eventId}")]
        public IActionResult Friends(int eventId)
        {
            return View();
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

    }
}