using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper;
using Hisab.Dapper.Repository;

namespace Hisab.BL
{
    public interface IEventManager
    {
        Task<int> CreateEvent(NewEventBO newNewEvent);

        Task<List<UserEventBO>> GetEvents(int userId);

        Task<List<UserEventBO>> GetAllEvents();

        Task<EventBO> GetEventById(int eventId);
    }

    

    public class EventManager : IEventManager
    {
        private IDbConnectionProvider _connectionProvider;
        private const int TotalAllowedEventsPerUser = 3;

        public EventManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<int> CreateEvent(NewEventBO newNewEvent)
        {
            //Check total allowed 
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                try
                {
                    var events = context.EventRepository.GetEventsForUser(newNewEvent.EventOwner.UserId);

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

        public Task<List<UserEventBO>> GetAllEvents()
        {
            throw new NotImplementedException();
        }

        public async Task<EventBO> GetEventById(int eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventById(eventId);


                return events;

            }
        }
    }

  
}
