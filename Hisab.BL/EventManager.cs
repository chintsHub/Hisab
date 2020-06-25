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
        Task<Guid> CreateEvent(NewEventBO newNewEvent);

        Task<List<UserEventBO>> GetEvents(Guid userId);

        Task<List<UserEventBO>> GetAllEvents();

        Task<EventBO> GetEventById(Guid eventId);

        Task<bool> UpdateEvenSettings(EventSettingsBO eventSettingsBO);

        Task<bool> ArchieveEvent(Guid eventId);

        Task<bool> CheckEventAccess(EventBO eventBo, string userName);
         

        
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
        public async Task<Guid> CreateEvent(NewEventBO newNewEvent)
        {
            //Check total allowed 
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                try
                {
                    var events = context.EventRepository.GetEventsForUser(newNewEvent.EventOwner.UserId);

                    //if (events.Count >= TotalAllowedEventsPerUser)
                    //    throw new HisabException("You have reached maximum number of allowed Events");


                    
                    newNewEvent.Id = Guid.NewGuid();
                    newNewEvent.EventOwner.EventId = newNewEvent.Id;
                    newNewEvent.CreateDateTime = DateTime.UtcNow;
                    newNewEvent.Status = EventStatus.Active;

                    if(context.EventRepository.CreateEvent(newNewEvent))
                    {
                        //add event and add owner as event friend
                        var friendResult = context.EventRepository.AddEventOwnerToEvent(newNewEvent.EventOwner);
                    }
                                    
                    

                    //create Accounts
                    var currentAccountId = context.EventRepository.CreateEventAccount(newNewEvent.Id,ApplicationAccountType.Cash);
                    //var expenseAccountId = context.EventRepository.CreateExpenseAccount(newNewEvent.Id);
                    //var owerAccount = context.EventRepository.CreateEventFriendAccount(newNewEvent.Id, friendId);

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

        public async Task<List<UserEventBO>> GetEvents(Guid userId)
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

        public async Task<EventBO> GetEventById(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventById(eventId);


                return events;

            }
        }

       

       

        public async Task<bool> UpdateEvenSettings(EventSettingsBO eventSettingsBO)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                
                
                var rows = context.EventRepository.UpdateEvent(eventSettingsBO);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

          

        public async Task<bool> CheckEventAccess(EventBO eventBo, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(await _userManager.IsInRoleAsync(user,"admin"))
            {
                return true;
            }

            var friend = eventBo.Friends.FirstOrDefault(x => x.Email.ToLower() == userName.ToLower());

            return friend != null;
        }

        

       

               

        public async Task<bool> ArchieveEvent(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var rows = context.EventRepository.ArchieveEvent(eventId);
                context.SaveChanges();

                if (rows == 1)
                    return true;

                return false;

            }
        }

       

        
    }

  
}
