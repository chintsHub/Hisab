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
        Task<List<EventInviteBO>> GetUserInvites(Guid userId);

        Task<int> JoinEvent(int eventFriendId, Guid appUserId);

        Task<ManagerResponse> InviteFriend(Guid eventId, string userEmail);

        Task<List<EventInviteBO>> GetPendingInvites(Guid eventId);
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

        public async Task<List<EventInviteBO>> GetUserInvites(Guid userId)
        {


            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetUserInvites(userId);

                return events;

            }
        }

        public async Task<int> JoinEvent(int eventFriendId, Guid appUserId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.JoinEvent(eventFriendId, appUserId);

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
                response.Messge = "User with this email address does not exist.";
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
                    response.Messge = "User with this email already exist in this event.";
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
                            response.Messge = "Invite send to join this event.";
                            response.Success = true;
                            return response;
                        }


                        response.Messge = "Failed to add friend.";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        response.Messge = "Invite has been send already. Awaiting friend to accept.";
                        response.Success = false;
                        return response;
                    }


                }
            }
        }

        public Task<List<EventInviteBO>> GetPendingInvites(Guid eventId)
        {
            throw new NotImplementedException();
        }
    }
}
