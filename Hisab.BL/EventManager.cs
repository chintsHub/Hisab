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
        Task<int> CreateEvent(EventBO newEvent);

        Task<List<UserEventBO>> GetEvents(int userId);

        Task<List<UserEventBO>> GetAllEvents();
    }

    public class EventManager : IEventManager
    {
        private IDbConnectionProvider _connectionProvider;
        private const int TotalAllowedEventsPerUser = 3;

        public EventManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<int> CreateEvent(EventBO newEvent)
        {
            //Check total allowed 
            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {
                var events = context.EventRepository.GetEventsForUser(newEvent.UserId);

                if (events.Count >= TotalAllowedEventsPerUser)
                    throw new HisabException("You have reached maximum number of allowed Events");

                newEvent.CreateDateTime = DateTime.Now;
                newEvent.LastModifiedDateTime = DateTime.Now;

                var id = context.EventRepository.CreateEvent(newEvent);

                context.SaveChanges();

                return id;
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
    }

  
}
