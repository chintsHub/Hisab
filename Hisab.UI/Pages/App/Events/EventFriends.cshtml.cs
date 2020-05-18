using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class EventFriendsModel : PageModel
    {
        [BindProperty]
        public InviteFriendVm NewFriend { get; set; }

        public string InviteFriendMessage { get; set; }
        
        private IEventInviteManager _eventInviteManager { get; set; }

        


        public EventFriendsModel(IEventInviteManager eventInviteManager)
        {
            _eventInviteManager = eventInviteManager;
        }
        public void OnGet(Guid Id)
        {
            NewFriend = new InviteFriendVm();

            NewFriend.EventId = Id;

        }

        public async Task<IActionResult> OnPost()
        {
            if(ModelState.IsValid)
            {
                var result = await _eventInviteManager.InviteFriend(NewFriend.EventId, NewFriend.FriendEmail);

                if(result.Success)
                {
                    InviteFriendMessage = result.Messge;
                }
                else
                {
                    ModelState.AddModelError("", result.Messge);
                }

            }

            return Page();
        }
    }
}