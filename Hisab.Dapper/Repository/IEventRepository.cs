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
        bool CreateEvent(NewEventBO newNewEvent);

        bool AddEventOwnerToEvent(NewEventFriendBO newNewEvent);

        List<UserEventBO> GetEventsForUser(Guid userId);

        EventBO GetEventById(Guid eventId);

        

        List<UserEventBO> GetAllEvents();

        

        int CreateEventFriend(Guid eventId, Guid userId, EventFriendStatus status);

        
        int UpdateEvent(EventSettingsBO eventSettingsBO);

        int ArchieveEvent(Guid eventId);

        int DisableFriend(int eventFriendId);

        int UpdateFriend(int kidsCount, int adultCount, int eventFriendId);
        int UpdateFriend(int kidsCount, int adultCount, string email, int eventFriendId);

        int CreateEventAccount(Guid eventId, ApplicationAccountType applicationAccountType);

        int CreateExpenseAccount(int eventId);

        int CreateEventFriendAccount(Guid eventId, int eventFriendId);

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

        public int CreateEventAccount(Guid eventId, ApplicationAccountType applicationAccountType)
        {
            Guid accountId = Guid.NewGuid();

            string command = $@"INSERT INTO [dbo].[EventAccount] ([AccountId] ,[EventId] ,[AccountTypeId])
                    VALUES (@{nameof(accountId)}, @{nameof(eventId)},@{nameof(applicationAccountType)});";

            var result = Connection.Execute(command,
                new
                {
                    accountId,
                    eventId,
                    applicationAccountType

                }, transaction: Transaction);

           
            return result;
        }

        public int CreateExpenseAccount(int eventId)
        {
            int? eventFriendId = null;
            int accountTypeId = 2;

            string command = $@"INSERT INTO [dbo].[EventAccount] ([Id] ,[EventFriendId] ,[AccountTypeId])
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

        public int CreateEventFriendAccount(Guid eventId, int eventFriendId)
        {
            int accountTypeId = 1;

            string command = $@"INSERT INTO [dbo].[EventAccount] ([Id] ,[EventFriendId] ,[AccountTypeId])
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
            bool IsFriendActive = true;

            string eventUserCommand = $@" INSERT INTO [dbo].[EventFriend] 
                                    ([UserId],[EventId] ,[Status], [IsFriendActive])
                    VALUES (@{nameof(newFriend.UserId)},
                            @{nameof(newFriend.EventId)}, 
                            @{nameof(newFriend.Status)}, @{nameof(IsFriendActive)});";

            int result = Connection.Execute(eventUserCommand,
                new
                {
                    newFriend.UserId,
                    newFriend.EventId,
                    newFriend.Status,
                    IsFriendActive

                }, transaction: Transaction);

            if (result == 1)
                return true;

            return false;
        }

        public List<UserEventBO> GetEventsForUser(Guid userId)
        {
             var result = Connection.Query<UserEventBO>($@"
                    select 
	                    e.Id,
	                    e.Name as EventName,
	                    e.CreateDate,
	                    e.Status as EventStatus,
	                    e.EventPic,
	                    e.UserId as OwnerUserId,
	                    u.NickName as OwnerName,

	                    ef.Status as EventFriendStatus
                    from 
	                    [dbo].[EventFriend] ef
	                    inner join Event e  on e.Id = ef.EventId
	                    inner join ApplicationUser u on u.Id = e.UserId
                    where
	                   ef.UserId = @{nameof(userId)}
                    order by e.CreateDate desc",
                 
                 new { userId }, Transaction);

            return result.ToList();
        }

        public List<UserEventBO> GetAllEvents()
        {
            var result = Connection.Query<UserEventBO>($@"
                    select 
	                    e.Id as Id,
	                    e.Name as EventName,
	                    u.NickName as NickName,
	                    e.Status
                    from 
	                [Event] e
	                    inner join [ApplicationUser] u on u.Id = e.UserId"

                , transaction:Transaction);

            return result.ToList();
        }

        public EventBO GetEventById(Guid eventId)
        {
            //https://dapper-tutorial.net/result-multi-mapping
            var sql = $@"
                    select 
	                    e.Id as Id,
                        e.Name as EventName,
                        e.Status as EventStatus,
                        e.EventPic as EventPicId,
                        e.CreateDate,

                        ef.UserId,
                        ef.EventId,
                        ef.Status as EventFriendStatus,
                        ef.IsFriendActive,

                        u.Email,
                        u.NickName,
                        u.AvatarId as Avatar
                        
                    from 
	                    [dbo].[Event] e
                        inner join [dbo].[EventFriend] ef on ef.EventId = e.Id
                        inner join [dbo].[ApplicationUser] u on u.Id = ef.UserId
	                where
                        e.Id = @{nameof(eventId)}";

            var eventDict = new Dictionary<Guid,EventBO>();

            var result = Connection.Query<EventBO, EventFriendBO, EventBO>(sql,
                (eventBO, eventFriend) =>
                {
                    EventBO eve;

                    if (!eventDict.TryGetValue(eventBO.Id, out eve))
                    {
                        eve = eventBO;
                        eve.Friends = new List<EventFriendBO>();
                        eve.Id = eventBO.Id;
                        
                        eventDict.Add(eve.Id,eve);
                    }
                    eve.Friends.Add(eventFriend);
                  
                    return eve;
                },
                new {eventId}, Transaction,splitOn: "UserId").ToList();

                      

            return result.FirstOrDefault();
        }

        public int CreateEventFriend(Guid eventId, Guid userId, EventFriendStatus status)
        {
            bool isActive = true;

            string command = $@"INSERT INTO [dbo].[EventFriend] ([UserId] , [EventId] , [Status] )
                    VALUES (@{nameof(userId)}, @{nameof(eventId)},@{nameof(status)})";

            var result = Connection.Execute(command,
                new
                {
                    eventId,
                    userId,
                    status

                }, transaction: Transaction);



            return result;

        }

        

        public int UpdateEvent(EventSettingsBO eventSettingsBO)
        {
            var eventRow = Connection.Execute($@"UPDATE [Event]
                    SET
                    [Name] = @{nameof(eventSettingsBO.EventName)},
                    [EventPic] = @{nameof(eventSettingsBO.SelectedEventImage)}
                    
                    WHERE [Id] = @{nameof(eventSettingsBO.EventId)}", 
                        new { eventSettingsBO.EventName, eventSettingsBO.EventId, eventSettingsBO.SelectedEventImage }, transaction: Transaction);

                foreach (var friend in eventSettingsBO.Friends)
                {
                    if (friend.EventFriendStatus != EventFriendStatus.EventAdmin)
                    {
                        var rows = Connection.Execute($@"UPDATE [EventFriend]
                        SET
                        [IsFriendActive] = @{nameof(friend.IsFriendActive)}
                                       
                        WHERE [UserId] = @{nameof(friend.UserId)} and [EventId] = @{nameof(friend.EventId)}",
                          new { friend.UserId, friend.EventId, friend.IsFriendActive }, transaction: Transaction);

                    }


                }

            return eventRow;
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

        public int ArchieveEvent(Guid eventId)
        {
            var rows = Connection.Execute($@"UPDATE [Event]
                    SET
                    [Status] = 2
                                        
                    WHERE [Id] = @{nameof(eventId)}", new { eventId }, transaction: Transaction);


            return rows;
        }

       
            
        
    }
}
