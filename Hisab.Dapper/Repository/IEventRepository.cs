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
        NewEventBO CreateEvent(NewEventBO newNewEvent);

        int AddEventOwnerToEvent(NewEventBO newNewEvent);

        List<UserEventBO> GetEventsForUser(Guid userId);

        EventBO GetEventById(int eventId);

        List<UserEventBO> GetAllEvents();

        int CreateEventFriend(EventFriendBO newEventFriend);

        int UpdateEvent(string newName, int eventId, EventStatus newStatus);

        int DisableFriend(int eventFriendId);

        int UpdateFriend(int kidsCount, int adultCount, int eventFriendId);
        int UpdateFriend(int kidsCount, int adultCount, string email, int eventFriendId);

        int CreateCurrentAccount(int eventId);

        int CreateExpenseAccount(int eventId);

        int CreateEventFriendAccount(int eventId, int eventFriendId);

    }

    internal class EventRepository : RepositoryBase, IEventRepository
    {
        public EventRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }
        public NewEventBO CreateEvent(NewEventBO newNewEvent)
        {
            string command = $@"INSERT INTO [dbo].[Event] ([UserId] ,[Name] ,[CreateDate],[LastModifiedDate],[Status])  
                    VALUES (@{nameof(newNewEvent.EventOwner.AppUserId)}, @{nameof(newNewEvent.EventName)},@{nameof(newNewEvent.CreateDateTime)}, @{nameof(newNewEvent.LastModifiedDateTime)},  @{nameof(newNewEvent.Status)} );
            SELECT CAST(SCOPE_IDENTITY() as int)";

            newNewEvent.Id = Connection.QuerySingle<int>(command,
                new
                {
                    newNewEvent.EventOwner.AppUserId,
                    newNewEvent.EventName,
                    newNewEvent.CreateDateTime,
                    newNewEvent.LastModifiedDateTime,
                    newNewEvent.Status
                }, transaction: Transaction);



            return newNewEvent;
        }

        public int CreateCurrentAccount(int eventId)
        {
            int? eventFriendId = null;
            int accountTypeId = 1;

            string command = $@"INSERT INTO [dbo].[EventAccount] ([EventId] ,[EventFriendId] ,[AccountTypeId])
                    VALUES (@{nameof(eventId)}, @{nameof(eventFriendId)},@{nameof(accountTypeId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int accountId = Connection.QuerySingle<int>(command,
                new
                {
                    eventId,
                    eventFriendId,
                    accountTypeId
                 
                }, transaction: Transaction);

           
            return accountId;
        }

        public int CreateExpenseAccount(int eventId)
        {
            int? eventFriendId = null;
            int accountTypeId = 2;

            string command = $@"INSERT INTO [dbo].[EventAccount] ([EventId] ,[EventFriendId] ,[AccountTypeId])
                    VALUES (@{nameof(eventId)}, @{nameof(eventFriendId)},@{nameof(accountTypeId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int accountId = Connection.QuerySingle<int>(command,
                new
                {
                    eventId,
                    eventFriendId,
                    accountTypeId

                }, transaction: Transaction);


            return accountId;
        }

        public int CreateEventFriendAccount(int eventId, int eventFriendId)
        {
            int accountTypeId = 1;

            string command = $@"INSERT INTO [dbo].[EventAccount] ([EventId] ,[EventFriendId] ,[AccountTypeId])
                    VALUES (@{nameof(eventId)}, @{nameof(eventFriendId)},@{nameof(accountTypeId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int accountId = Connection.QuerySingle<int>(command,
                new
                {
                    eventId,
                    eventFriendId,
                    accountTypeId

                }, transaction: Transaction);


            return accountId;
        }

        public int AddEventOwnerToEvent(NewEventBO newNewEvent)
        {
            string eventUserCommand = $@" INSERT INTO [dbo].[EventFriend] 
                                    ([EventId] ,[Email] , [NickName] ,[Status] ,[AppUserId] ,[AdultCount] ,[KidsCount])
                    VALUES (@{nameof(newNewEvent.Id)}, @{nameof(newNewEvent.EventOwner.Email)}, @{nameof(newNewEvent.EventOwner.NickName)}, 
                                    @{nameof(newNewEvent.EventOwner.Status)}, @{nameof(newNewEvent.EventOwner.AppUserId)}, @{nameof(newNewEvent.EventOwner.AdultCount)},@{nameof(newNewEvent.EventOwner.KidsCount)}); 
                                    SELECT CAST(SCOPE_IDENTITY() as int)";

            int friendId = Connection.QuerySingle<int>(eventUserCommand,
                new
                {
                    newNewEvent.Id,
                    newNewEvent.EventOwner.Email,
                    newNewEvent.EventOwner.NickName,
                    newNewEvent.EventOwner.Status,
                    newNewEvent.EventOwner.AppUserId,
                    newNewEvent.EventOwner.AdultCount,
                    newNewEvent.EventOwner.KidsCount
                }, transaction: Transaction);

            return friendId;
        }

        public List<UserEventBO> GetEventsForUser(Guid userId)
        {
             var result = Connection.Query<UserEventBO>($@"
                    select 
	                e.Id as EventId,
	                e.Name as EventName,
	                u.NickName as NickName,
                    e.Status
	               
                from 
	                [dbo].[EventFriend] ef
	                inner join [Event] e on ef.EventId = e.Id
	                inner join [ApplicationUser] u on u.Id = ef.AppUserId
                where
                    ef.status in (1,5) and
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
	                    u.NickName as NickName,
	                    e.Status
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
                        e.Status,

                        ef.EventFriendId as EventFriendId,
                        ef.Email,
                        ef.NickName as NickName,
                        ef.Email,
                        ef.Status,
                        ef.AdultCount,
                        ef.KidsCount,
                        ef.AppUserId
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
                               @{nameof(newEventFriend.AppUserId)}, @{nameof(newEventFriend.AdultCount)},@{nameof(newEventFriend.KidsCount)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            newEventFriend.EventFriendId = Connection.QuerySingle<int>(command,
                new
                {
                    newEventFriend.EventId,
                    newEventFriend.Email,
                    newEventFriend.NickName,
                    newEventFriend.Status,
                    newEventFriend.AppUserId,
                    newEventFriend.AdultCount,
                    newEventFriend.KidsCount
                    
                }, transaction: Transaction);



            return newEventFriend.EventFriendId;
        }

        public int UpdateEvent(string newName, int eventId, EventStatus newStatus)
        {
            var rows = Connection.Execute($@"UPDATE [Event]
                    SET
                    [Name] = @{nameof(newName)},
                    [Status] = @{nameof(newStatus)}
                    
                    WHERE [Id] = @{nameof(eventId)}", new { newName, eventId, newStatus }, transaction: Transaction);


            return rows;
        }

        public int DisableFriend(int eventFriendId)
        {
            var rows = Connection.Execute($@"UPDATE [EventFriend]
                    SET
                      [Status] = 6
                    
                    WHERE [EventFriendId] = @{nameof(eventFriendId)}", new { eventFriendId }, transaction: Transaction);


            return rows;
        }

        public int UpdateFriend(int kidsCount, int adultCount, int eventFriendId)
        {
            var rows = Connection.Execute($@"UPDATE [EventFriend]
                    SET
                      [kidsCount] = @{nameof(kidsCount)},
                      [adultCount] = @{nameof(adultCount)}
                    
                    WHERE [EventFriendId] = @{nameof(eventFriendId)}", new { kidsCount, adultCount, eventFriendId }, transaction: Transaction);


            return rows;
        }

        public int UpdateFriend(int kidsCount, int adultCount, string email, int eventFriendId)
        {
            var rows = Connection.Execute($@"UPDATE [EventFriend]
                    SET
                      [kidsCount] = @{nameof(kidsCount)},
                      [adultCount] = @{nameof(adultCount)},
                      [Email] = @{nameof(email)}
                    
                    WHERE [EventFriendId] = @{nameof(eventFriendId)}", new { kidsCount, adultCount, email, eventFriendId }, transaction: Transaction);


            return rows;
        }
    }
}
