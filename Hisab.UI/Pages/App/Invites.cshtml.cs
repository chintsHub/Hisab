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

namespace Hisab.UI
{
    public class InvitesModel : PageModel
    {
        public List<EventCardVm> Events { get; set; }

        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;

        public InvitesModel(IEventManager eventManager, UserManager<ApplicationUser> userManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;

            Events = new List<EventCardVm>();
        }
        
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var invites = await _eventManager.GetUserInvites(user.Id);

            foreach (var eventBo in invites)
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
    }
}