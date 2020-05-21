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
        [BindProperty]
        public List<EventCardVm> Events { get; set; }

        private IEventInviteManager _eventInviteManager;
        private UserManager<ApplicationUser> _userManager;

        public string InviteSuccessMessage { get; set; }

        public string InviteErrorMessage { get; set; }

        public InvitesModel(IEventInviteManager eventInviteManager, UserManager<ApplicationUser> userManager)
        {
            _eventInviteManager = eventInviteManager;
            _userManager = userManager;

            Events = new List<EventCardVm>();
        }
        
        public async Task<IActionResult> OnGet()
        {
            await LoadInvites();

            return Page();
        }

        private async Task LoadInvites()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var invites = await _eventInviteManager.GetUserInvites(user.Id);

            foreach (var inviteBo in invites)
            {
                Events.Add(new EventCardVm()
                {
                    EventId = inviteBo.EventId,
                    EventName = inviteBo.EventName,
                    CreatedUserNickName = inviteBo.EventOwnerName,
                    EventMessage = "Event Created by: " + inviteBo.EventOwnerName,
                    EventImagePath = HisabImageManager.GetEventImages()
                                    .Where<HisabImage>(x => x.Id == inviteBo.EventPic).FirstOrDefault().ImagePath
                });
            }
        }

        public async Task<IActionResult> OnPostInviteDelete(Guid EventId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _eventInviteManager.DeleteInvite(EventId, user.Id);

            if(result)
            {
                InviteSuccessMessage = "Sucessfully deleted the invite.";
                await LoadInvites();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostInviteJoin(Guid EventId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _eventInviteManager.JoinInvite(EventId, user.Id);

            if(result.Success)
            {
                InviteSuccessMessage = result.Messge;
            }
            else
            {
                InviteErrorMessage =  result.Messge;
            }

            await LoadInvites();
            return Page();
        }
    }
}