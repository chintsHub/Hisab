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
        int CreateEvent(NewEventBO newNewEvent);

        List<UserEventBO> GetEventsForUser(int userId);

        EventBO GetEventById(int eventId);

        List<UserEventBO> GetAllEvents();

        int CreateEventFriend(EventFriendBO newEventFriend);

        int UpdateEventName(string newName, int eventId);

    }

    internal class EventRepository : RepositoryBase, IEventRepository
    {
        public EventRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }
        public int CreateEvent(NewEventBO newNewEvent)
        {
            string command = $@"INSERT INTO [dbo].[Event] ([UserId] ,[Name] ,[CreateDate],[LastModifiedDate])  
                    VALUES (@{nameof(newNewEvent.EventOwner.UserId)}, @{nameof(newNewEvent.EventName)},@{nameof(newNewEvent.CreateDateTime)}, @{nameof(newNewEvent.LastModifiedDateTime)} );
            SELECT CAST(SCOPE_IDENTITY() as int)";

            newNewEvent.Id = Connection.QuerySingle<int>(command,
                new
                {
                    newNewEvent.EventOwner.UserId, newNewEvent.EventName, newNewEvent.CreateDateTime,
                    newNewEvent.LastModifiedDateTime
                }, transaction: Transaction);

            AddEventOwnerToEvent(newNewEvent);

            return newNewEvent.Id;
        }

        private void AddEventOwnerToEvent(NewEventBO newNewEvent)
        {
            string eventUserCommand = $@" INSERT INTO [dbo].[EventFriend] 
                                    ([EventId] ,[Email] , [NickName] ,[Status] ,[AppUserId] ,[AdultCount] ,[KidsCount])
                    VALUES (@{nameof(newNewEvent.Id)}, @{nameof(newNewEvent.EventOwner.Email)}, @{nameof(newNewEvent.EventOwner.NickName)}, 
                                    @{nameof(newNewEvent.EventOwner.Status)}, @{nameof(newNewEvent.EventOwner.UserId)}, @{nameof(newNewEvent.EventOwner.AdultCount)},@{nameof(newNewEvent.EventOwner.KidsCount)}); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

            Connection.Execute(eventUserCommand,
                new
                {
                    newNewEvent.Id,
                    newNewEvent.EventOwner.Email,
                    newNewEvent.EventOwner.NickName,
                    newNewEvent.EventOwner.Status,
                    newNewEvent.EventOwner.UserId,
                    newNewEvent.EventOwner.AdultCount,
                    newNewEvent.EventOwner.KidsCount
                }, transaction: Transaction);
        }

        public List<UserEventBO> GetEventsForUser(int userId)
        {
             var result = Connection.Query<UserEventBO>($@"
                    select 
	                e.Id as EventId,
	                e.Name as EventName,
	                u.NickName as NickName
	               
                from 
	                [dbo].[EventFriend] ef
	                inner join [Event] e on ef.EventId = e.Id
	                inner join [ApplicationUser] u on u.Id = ef.AppUserId
                where
                 ef.AppUserId = @{nameof(userId)}",
                 
                 new { userId }, Transaction);

            return result.ToList();
        }

        public List<UserEventBO> GetAllEvents()
        {
            var result = Connection.Query<UserEventBO>($@"
                    select 
	                    e.Id as EventId,
	                    e.Name as EventName,
	                    u.NickName as NickName
	               
                    from 
	                [Event] e
	                    inner join [ApplicationUser] u on u.Id = e.UserId"

                , transaction:Transaction);

            return result.ToList();
        }

        public EventBO GetEventById(int eventId)
        {
            //https://dapper-tutorial.net/result-multi-mapping
            var sql = $@"
                    select 
	                    e.Id as EventId,
                        e.Name as EventName,
                             
                        ef.EventFriendId as EventFriendId,
                        ef.Email,
                        ef.NickName as NickName,
                        ef.Email,
                        ef.Status,
                        ef.AdultCount,
                        ef.KidsCount
                    from 
	                    [dbo].[Event] e
                        inner join [dbo].[EventFriend] ef on ef.EventId = e.Id
	                where
                        e.Id = @{nameof(eventId)}";

            var eventDict = new Dictionary<int,EventBO>();

            var result = Connection.Query<EventBO, EventFriendBO, EventBO>(sql,
                (eventBO, eventFriend) =>
                {
                    EventBO eve;

                    if (!eventDict.TryGetValue(eventBO.EventId, out eve))
                    {
                        eve = eventBO;
                        eve.Friends = new List<EventFriendBO>();
                        eve.EventId = eventBO.EventId;
                        eventDict.Add(eve.EventId,eve);
                    }
                    eve.Friends.Add(eventFriend);
                  
                    return eve;
                },
                new {eventId}, Transaction,splitOn: "EventFriendId").ToList();

                      

            return result.FirstOrDefault();
        }

        public int CreateEventFriend(EventFriendBO newEventFriend)
        {
            string command = $@"INSERT INTO [dbo].[EventFriend] ([EventId] ,[Email] ,[NickName] ,[Status] ,[AppUserId] ,[AdultCount] ,[KidsCount])
                    VALUES (@{nameof(newEventFriend.EventId)}, @{nameof(newEventFriend.Email)},@{nameof(newEventFriend.NickName)}, @{nameof(newEventFriend.Status)},
                               @{nameof(newEventFriend.UserId)}, @{nameof(newEventFriend.AdultCount)},@{nameof(newEventFriend.KidsCount)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            newEventFriend.EventFriendId = Connection.QuerySingle<int>(command,
                new
                {
                    newEventFriend.EventId,
                    newEventFriend.Email,
                    newEventFriend.NickName,
                    newEventFriend.Status,
                    newEventFriend.UserId,
                    newEventFriend.AdultCount,
                    newEventFriend.KidsCount
                    
                }, transaction: Transaction);



            return newEventFriend.EventFriendId;
        }

        public int UpdateEventName(string newName, int eventId)
        {
            var rows = Connection.Execute($@"UPDATE [Event]
                    SET
                    [Name] = @{nameof(newName)}
                    
                    
                    WHERE [Id] = @{nameof(eventId)}", new { newName, eventId }, transaction: Transaction);


            return rows;
        }
    }
}
