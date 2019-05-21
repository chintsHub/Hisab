using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.Dapper.Model;

namespace Hisab.Dapper.Repository
{
    public interface IEventRepository
    {
        int CreateEvent(EventBO newEvent);

        List<UserEventBO> GetEventsForUser(int userId);

    }

    internal class EventRepository : RepositoryBase, IEventRepository
    {
        public EventRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }
        public int CreateEvent(EventBO newEvent)
        {
            string command = $@"INSERT INTO [dbo].[Event] ([UserId] ,[Name] ,[CreateDate],[LastModifiedDate])  
                    VALUES (@{nameof(newEvent.UserId)}, @{nameof(newEvent.EventName)},@{nameof(newEvent.CreateDateTime)}, @{nameof(newEvent.LastModifiedDateTime)} );
            SELECT CAST(SCOPE_IDENTITY() as int)";

            var eventId = Connection.QuerySingle<int>(command,
                new
                {
                    newEvent.UserId, newEvent.EventName, newEvent.CreateDateTime,
                    newEvent.LastModifiedDateTime
                }, transaction: Transaction);

            int adultCount = 1;
            int kidsCount = 0;
            string eventUserCommand = $@"INSERT INTO [dbo].[EventUser]([EventId],[UserId],[AdultCount],[KidCount])  
                    VALUES (@{nameof(eventId)}, @{nameof(newEvent.UserId)}, @{nameof(adultCount)},@{nameof(kidsCount)});  ";

            Connection.Execute(eventUserCommand,
                new
                {
                    eventId, newEvent.UserId, adultCount, kidsCount
                }, transaction: Transaction);

            return eventId;
        }

        public List<UserEventBO> GetEventsForUser(int userId)
        {
             var result = Connection.Query<UserEventBO>($@"
                    select 
	                e.Id as EventId,
	                e.Name as EventName,
	                u.Id as userId,
	                u.Email,
	                u.NickName,
	                u1.Email as eventOwner
                from 
	                [dbo].[EventUser] eu
	                inner join [Event] e on eu.EventId = e.Id
	                inner join ApplicationUser u on u.Id = eu.UserId
	                inner join ApplicationUser u1 on u1.Id = e.UserId
                where
                 eu.UserId = @{nameof(userId)}", new { userId }, Transaction);

            return result.ToList();
        }

      
    }
}
