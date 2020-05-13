﻿using System;
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
        bool CreateEvent(NewEventBO newNewEvent);

        bool AddEventOwnerToEvent(NewEventFriendBO newNewEvent);

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
        public bool CreateEvent(NewEventBO newNewEvent)
        {
            var eventPic = newNewEvent.EventPic.Id;

            string command = $@"INSERT INTO [dbo].[Event] ([Id], [UserId] ,[Name] ,[CreateDate],[Status],[EventPic])  
                    VALUES (
                        @{nameof(newNewEvent.Id)},
                        @{nameof(newNewEvent.EventOwner.UserId)}, 
                        @{nameof(newNewEvent.EventName)},
                        @{nameof(newNewEvent.CreateDateTime)}, 
                        @{nameof(newNewEvent.Status)},
                        @{nameof(eventPic)});
            ";

            var result = Connection.Execute(command,
                new
                {
                    newNewEvent.Id,
                    newNewEvent.EventOwner.UserId,
                    newNewEvent.EventName,
                    newNewEvent.CreateDateTime,
                    newNewEvent.Status,
                    eventPic

                }, transaction: Transaction);


            if (result == 1)
                return true;

            return false;
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

        public bool AddEventOwnerToEvent(NewEventFriendBO newFriend)
        {
            string eventUserCommand = $@" INSERT INTO [dbo].[EventFriend] 
                                    ([UserId],[EventId] ,[Status])
                    VALUES (@{nameof(newFriend.UserId)},
                            @{nameof(newFriend.EventId)}, 
                            @{nameof(newFriend.Status)});";

            int result = Connection.Execute(eventUserCommand,
                new
                {
                    newFriend.UserId,
                    newFriend.EventId,
                    newFriend.Status
                    
                    
                }, transaction: Transaction);

            if (result == 1)
                return true;

            return false;
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
	                inner join [ApplicationUser] u on u.Id = ef.UserId
                where
                    ef.status in (1,5) and
                 ef.UserId = @{nameof(userId)}",
                 
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
