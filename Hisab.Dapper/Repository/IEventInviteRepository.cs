using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;

namespace Hisab.Dapper.Repository
{
    public interface IEventInviteRepository
    {
        List<UserEventInviteBO> GetUserInvites(Guid userId);

        

        UserEventInviteBO GetInvite(Guid eventId, Guid userId);

        int InviteFriend(Guid eventId, Guid userId, InviteStatus status);

        List<UserEventInviteBO> GetPendingInvites(Guid eventId);

        List<ApplicationUser> GetRecommendedFriends(Guid userId, Guid currentEventId);

        bool DeleteInvite(Guid eventId, Guid userId);

        int JoinEvent(Guid eventId, Guid userId, EventFriendStatus status);
    }

    internal class EventInviteRepository : RepositoryBase, IEventInviteRepository
    {

        public EventInviteRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }
        public List<UserEventInviteBO> GetUserInvites(Guid userId)
        {
            var result = Connection.Query<UserEventInviteBO>($@"
                       select 
	                        ei.UserId,
                            ei.EventId,
                            e.Name as EventName,
                            e.EventPic,
                            u.NickName as EventOwnerName
	                        
                            
                      from
                            [dbo].[EventInvites] ei
                            inner join [dbo].[Event] e on e.Id = ei.EventId
                            inner join [dbo].[ApplicationUser] u on u.Id = e.UserId -- joining owner

                        where
                            ei.InviteStatus = 1
                            and ei.UserId  =  @{nameof(userId)}
                            
				       ",

                new { userId }, Transaction);

            return result.ToList();
        }

       

        public int InviteFriend(Guid eventId, Guid userId, InviteStatus status)
        {


            string command = $@"INSERT INTO [dbo].[EventInvites] ([UserId] , [EventId] , [InviteStatus] )
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

        public UserEventInviteBO GetInvite(Guid eventId, Guid userId)
        {
            var result = Connection.Query<UserEventInviteBO>($@"
                    select 
	                    i.EventId,
                        i.UserId,
                        i.InviteStatus
                    from 
	                [EventInvites] i

                    where
                        i.EventId = @{nameof(eventId)} and i.UserId = @{nameof(userId)} "

                , new { eventId, userId }, transaction: Transaction);

            return result.FirstOrDefault();
        }

        public List<UserEventInviteBO> GetPendingInvites(Guid eventId)
        {
            var result = Connection.Query<UserEventInviteBO>($@"
                       select 
	                        u.NickName,
                            u.AvatarId,
                            u.Email,
                            ei.UserId,
                            ei.EventId
                      from [EventInvites] ei
                          inner join ApplicationUser u on u.Id = ei.UserId

                      where ei.EventId = @{nameof(eventId)}",

               new { eventId }, Transaction);

            return result.ToList();
        }

        public List<ApplicationUser> GetRecommendedFriends(Guid userId, Guid currentEventId)
        {
            var result = Connection.Query<ApplicationUser>($@"
                   select
	                    u.Id,
	                    u.NickName,
	                    u.Email,
	                    u.AvatarId  
                    from
                     ApplicationUser u
                     inner join

                        (
                        select
	                        ef.EventId,
	                        ef.UserId
                        from
	                        [dbo].[Event] e
	                        inner join [dbo].[EventFriend] ef on e.Id = ef.EventId
	
	                        inner join
                        (select
	                          distinct ef.EventId
                        from 
	                           [dbo].[EventFriend] ef     
                         where
                               ef.EventId <> @{nameof(currentEventId)}
	                           and ef.UserId =  @{nameof(userId)} ) OtherEvents
	                        on OtherEvents.EventId = e.Id  
                        where
	                           ef.UserId <>  @{nameof(userId)}   ) OtherEventsWithOtherUsers   

                        on u.Id = OtherEventsWithOtherUsers.UserId

                        where
                          OtherEventsWithOtherUsers.UserId not in (select e.UserId from [dbo].[EventFriend] e where e.EventId = @{nameof(currentEventId)}) ",

                new { currentEventId, userId }, Transaction);

            return result.Take(20).ToList();
        }

        public bool DeleteInvite(Guid eventId, Guid userId)
        {
            string command = $@"DELETE from  [dbo].[EventInvites]
                        Where UserId = @{nameof(userId)} and EventId = @{nameof(eventId)}   ";

            var result = Connection.Execute(command,
                new
                {
                    userId,
                    eventId

                }, transaction: Transaction);


            if (result > 0)
                return true;
            
            
            return false;
        }

        public int JoinEvent(Guid eventId, Guid userId, EventFriendStatus status)
        {
            bool IsFriendActive = true;

            string command = $@"INSERT INTO [dbo].[EventFriend] ([UserId] , [EventId] , [Status], [IsFriendActive] )
                    VALUES (@{nameof(userId)}, @{nameof(eventId)},@{nameof(status)}, @{nameof(IsFriendActive)})";

            var result = Connection.Execute(command,
                new
                {
                    eventId,
                    userId,
                    status,
                    IsFriendActive
                }, transaction: Transaction);



            return result;
        }
    }
}
