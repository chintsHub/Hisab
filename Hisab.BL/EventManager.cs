using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.Dapper.Repository;
using Microsoft.AspNetCore.Identity;

namespace Hisab.BL
{
    public interface IEventManager
    {
        Task<int> CreateEvent(NewEventBO newNewEvent);

        Task<List<UserEventBO>> GetEvents(int userId);

        Task<List<UserEventBO>> GetAllEvents();

        Task<EventBO> GetEventById(int eventId);

        Task<bool> CreateEventFriend(EventFriendBO newEventFriend);

        Task<bool> UpdateEvent(string newName, int eventId, EventStatus newStatus);

        Task<bool> DisableFriend(int eventFriendId);

        Task<bool> UpdateFriend(EventFriendBO eventFriendBo);

        Task<bool> CanAccessEvent(int eventId, int userId);

        bool CheckEventAccess(EventBO eventBo, int userId);
    }

    

    public class EventManager : IEventManager
    {
        private IDbConnectionProvider _connectionProvider;
        private UserManager<ApplicationUser> _userManager;

        private const int TotalAllowedEventsPerUser = 3;

        public EventManager(IDbConnectionProvider connectionProvider, UserManager<ApplicationUser> userManager)
        {
            _connectionProvider = connectionProvider;
            _userManager = userManager;
        }
        public async Task<int> CreateEvent(NewEventBO newNewEvent)
        {
            //Check total allowed 
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                try
                {
                    var events = context.EventRepository.GetEventsForUser(newNewEvent.EventOwner.UserId.Value);

                    if (events.Count >= TotalAllowedEventsPerUser)
                        throw new HisabException("You have reached maximum number of allowed Events");


                    newNewEvent.Id = context.EventRepository.CreateEvent(newNewEvent);



                    context.SaveChanges();

                    return newNewEvent.Id;
                }
                catch (Exception ex)
                {
                    if (context != null)
                    {
                        context.Dispose();
                    }

                    throw ex;
                }
             
            }

             
        }

        public async Task<List<UserEventBO>> GetEvents(int userId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventsForUser(userId);

                
                return events;
                
            }
        }

        public async Task<List<UserEventBO>> GetAllEvents()
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetAllEvents();


                return events;

            }
        }

        public async Task<EventBO> GetEventById(int eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventById(eventId);


                return events;

            }
        }

        public async Task<bool> CreateEventFriend(EventFriendBO newEventFriend)
        {
            
            //find user
            var user = await _userManager.FindByEmailAsync(newEventFriend.Email);

            if (user == null)
            {
                newEventFriend.Status = EventFriendStatus.EventFriend;
                newEventFriend.UserId = null;
            }
            else
            {
                newEventFriend.Status = user.EmailConfirmed ? EventFriendStatus.PendingAcceptance : EventFriendStatus.PendingRegistration;
                newEventFriend.UserId = user.Id;
            }

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var friend = context.EventRepository.CreateEventFriend(newEventFriend);
                context.SaveChanges();

                if (friend > 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> UpdateEvent(string newName, int eventId, EventStatus newStatus)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.EventRepository.UpdateEvent(newName,eventId, newStatus);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> DisableFriend(int eventFriendId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.EventRepository.DisableFriend(eventFriendId);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

        public async Task<bool> UpdateFriend(EventFriendBO eventFriendBo)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                int rows =0;
                if (eventFriendBo.Status == EventFriendStatus.EventOwner || eventFriendBo.Status == EventFriendStatus.EventJoined
                                                                         || eventFriendBo.Status == EventFriendStatus.PendingAcceptance
                                                                         || eventFriendBo.Status == EventFriendStatus.PendingRegistration)
                {
                    rows = context.EventRepository.UpdateFriend(eventFriendBo.KidsCount, eventFriendBo.AdultCount, eventFriendBo.EventFriendId);
                    context.SaveChanges();
                }

                if (eventFriendBo.Status == EventFriendStatus.EventFriend)
                {
                    rows = context.EventRepository.UpdateFriend(eventFriendBo.KidsCount, eventFriendBo.AdultCount, eventFriendBo.Email, eventFriendBo.EventFriendId);
                    context.SaveChanges();
                }

                if (rows == 1)
                    return true;

               
                return false;

            }
        }

        public async Task<bool> CanAccessEvent(int eventId, int userId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var eventBo = context.EventRepository.GetEventById(eventId);

                return CheckEventAccess(eventBo, userId);



            }

           
        }

        public bool CheckEventAccess(EventBO eventBo, int userId)
        {
            var friend = eventBo.Friends.FirstOrDefault(x => x.UserId != null && x.UserId.Value == userId);

            return friend != null;
        }
    }

  
}
