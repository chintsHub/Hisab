using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hisab.Common.BO;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hisab.BL
{
    public interface IEventInviteManager
    {
        Task<List<UserEventInviteBO>> GetUserInvites(Guid userId);

        Task<ManagerResponse> JoinInvite(Guid eventId, Guid userId);


        Task<ManagerResponse> InviteFriend(Guid eventId, string userEmail);

        Task<List<ManagerResponse>> InviteFriends(List<NewInviteBO> invites);

        Task<List<UserEventInviteBO>> GetPendingInvites(Guid eventId);

        Task<List<ApplicationUser>> GetRecommendedFriends(Guid userId, Guid currentEventId);

        Task<bool> DeleteInvite(Guid eventId, Guid userId);

        
    }

    public class EventInviteManager : IEventInviteManager
    {
        private IDbConnectionProvider _connectionProvider;
        private UserManager<ApplicationUser> _userManager;
        private IEventManager _eventManager;

        public EventInviteManager(IDbConnectionProvider connectionProvider, UserManager<ApplicationUser> userManager, IEventManager eventManager)
        {
            _connectionProvider = connectionProvider;
            _userManager = userManager;
            _eventManager = eventManager;
        }

        public async Task<List<UserEventInviteBO>> GetUserInvites(Guid userId)
        {


            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetUserInvites(userId);

                return events;

            }
        }

       

        public async Task<ManagerResponse> InviteFriend(Guid eventId, string userEmail)
        {
            var response = new ManagerResponse();

            //find user
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                response.Messge = $"User with the email address {userEmail} is not registered with Hisaab.";
                response.Success = false;
                return response;
            }
            else
            {
                //Load event and check Friend exist
                var eve = await _eventManager.GetEventById(eventId);
                var friend = eve.Friends.Where(x => x.Email.ToLower() == userEmail.ToLower()).FirstOrDefault();

                if (friend != null)
                {
                    response.Messge = $"User with the email address {userEmail} already exist in this event.";
                    response.Success = false;
                    return response;
                }

                //Check if Invite has already been sent


                using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
                {
                    var invite = context.EventInviteRepository.GetInvite(eventId, user.Id);

                    if (invite == null)
                    {
                        var newInvite = context.EventInviteRepository.InviteFriend(eventId, user.Id, InviteStatus.RequestPending);

                        //create account
                        //var accountId = context.EventRepository.CreateEventFriendAccount(newEventFriend.EventId, friend);

                        context.SaveChanges();

                        if (newInvite > 0)
                        {
                            response.Messge = $"Invite send to the user with email address {userEmail} join this event.";
                            response.Success = true;
                            return response;
                        }


                        response.Messge = $"Failed to add friend.Email: {userEmail}";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        response.Messge = $"Invite has been send already to the friend {userEmail}. Awaiting friend to accept.";
                        response.Success = false;
                        return response;
                    }


                }
            }
        }

        public async Task<List<UserEventInviteBO>> GetPendingInvites(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetPendingInvites(eventId);

                return events;

            }
        }

        public async Task<List<ApplicationUser>> GetRecommendedFriends(Guid userId, Guid currentEventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetRecommendedFriends(userId, currentEventId);

                
                return events;

            }
        }

        public async Task<bool> DeleteInvite(Guid eventId, Guid userId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var result = context.EventInviteRepository.DeleteInvite(eventId, userId);

                return result;

            }
        }

        public async Task<ManagerResponse> JoinInvite(Guid eventId, Guid userId)
        {
            var retVal = new ManagerResponse();
            retVal.Success = false;

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var result = context.EventInviteRepository.DeleteInvite(eventId, userId);

                if(!result)
                {
                    retVal.Messge = "Incorrect invite.";
                }

                context.EventInviteRepository.JoinEvent(eventId, userId, EventFriendStatus.EventFriend);

                context.SaveChanges();

                retVal.Messge = "You have successfully joined the event. The event will now appear on your Events page.";
                retVal.Success = true;

            }

            return retVal;
        }

        public async Task<List<ManagerResponse>> InviteFriends(List<NewInviteBO> invites)
        {
            var retVal = new List<ManagerResponse>();

            foreach(var invite in invites)
            {
                var inviteResult = await InviteFriend(invite.EventId, invite.UserEmail);
                retVal.Add(inviteResult);
            }

            return retVal;
        }
    }
}
