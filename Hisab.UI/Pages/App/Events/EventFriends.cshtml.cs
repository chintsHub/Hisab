using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.AWS;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Hisab.UI
{
    [Authorize(Roles = "App User, Admin")]
    public class EventFriendsModel : PageModel
    {
        [BindProperty]
        public InviteFriendVm NewFriend { get; set; }

        public string InviteFriendMessage { get; set; }

        public List<string> InviteFriendMessages { get; set; }


        private IEventInviteManager _eventInviteManager { get; set; }
        private UserManager<ApplicationUser> _userManager { get; set; }

        private IEventManager _eventManager;
        private IEmailService _emailService;

        public List<UserEventInviteVM> PendingRequests { get; set; }
        
        [BindProperty]
        public List<InviteApplicationUserVM> RecommendedFriends { get; set; }


        public EventFriendsModel(IEventInviteManager eventInviteManager, UserManager<ApplicationUser> userManager, IEventManager eventManager, IEmailService emailService)
        {
            _eventInviteManager = eventInviteManager;
            _userManager = userManager;
            _eventManager = eventManager;
            _emailService = emailService;

            PendingRequests = new List<UserEventInviteVM>();
            RecommendedFriends = new List<InviteApplicationUserVM>();
            InviteFriendMessages = new List<string>();
        }
        public async Task<IActionResult> OnGet(Guid Id)
        {
            NewFriend = new InviteFriendVm();
            NewFriend.EventId = Id;

            
            var eve = await _eventManager.GetEventById(Id);
            this.ViewData.Add("EventTitle", eve.EventName);
            this.ViewData.Add("EventId", eve.Id);

            if (await _eventManager.CheckEventAccess(eve, User.Identity.Name))
            {
                //Get loggedin user
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                await LoadPendingRequest(NewFriend.EventId);

                await LoadRecommendedFriends(user.Id, NewFriend.EventId);

                return Page();
            }

            throw new UnauthorizedAccessException();

        }

        private async Task LoadPendingRequest(Guid Id)
        {
            //Get Pending friend requests
            var pendingRequests = await _eventInviteManager.GetPendingInvites(Id);

            foreach (var pendingRequest in pendingRequests)
            {
                PendingRequests.Add(new UserEventInviteVM()
                {
                    EventId = pendingRequest.EventId,
                    InviteStatus = pendingRequest.InviteStatus,
                    UserId = pendingRequest.UserId,
                    NickName = pendingRequest.NickName,
                    Avatar = HisabImageManager.GetAvatar(pendingRequest.AvatarId)
                });
            }
        }

        private async Task LoadRecommendedFriends(Guid userId, Guid eventId)
        {
            //Get recommended friends
            var recommendedFriends = await _eventInviteManager.GetRecommendedFriends(userId, eventId);

            foreach (var friend in recommendedFriends)
            {
                if(RecommendedFriends.FirstOrDefault(x => x.Id == friend.Id) == null)
                {
                    RecommendedFriends.Add(new InviteApplicationUserVM()
                    {
                        Id = friend.Id,
                        NickName = friend.NickName,
                        UserName = friend.Email,
                        Avatar = HisabImageManager.GetAvatar((AvatarEnum)friend.AvatarId),
                        Checked = false,
                        EventId = eventId
                    });
                }
                
                
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if(ModelState.IsValid)
            {
                var fromUser = await _userManager.FindByNameAsync(User.Identity.Name);
                var result = await _eventInviteManager.InviteFriend(NewFriend.EventId, NewFriend.FriendEmail, fromUser.NickName);

                if(result.Success)
                {
                    InviteFriendMessage = result.Messge;
                                
                   
                }
                else
                {
                    ModelState.AddModelError("", result.Messge);
                }

            }

            //Get loggedin user
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await LoadPendingRequest(NewFriend.EventId);

            await LoadRecommendedFriends(user.Id, NewFriend.EventId);

            return Page();
        }

        public async Task<IActionResult> OnPostSendInvites()
        {
            var invites = new List<NewInviteBO>();
            var fromUser = await _userManager.FindByNameAsync(User.Identity.Name);

            foreach (var friend in RecommendedFriends)
            {
                if(friend.Checked)
                {
                    invites.Add(new NewInviteBO() { EventId = friend.EventId, UserEmail = friend.UserName, LoggedInUser = fromUser.NickName });
                    NewFriend.EventId = friend.EventId; // eventId is going to be same
                }
            }

            if (invites.Count > 0)
            {
                var response = await _eventInviteManager.InviteFriends(invites);

                foreach (var r in response)
                {
                    InviteFriendMessages.Add(r.Messge);
                }
           
            }
            else
                InviteFriendMessages.Add("You must select a friend to add");


            await LoadPendingRequest(NewFriend.EventId);
            return Page();
        }
    }
}