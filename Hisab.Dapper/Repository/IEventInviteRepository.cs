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
        List<EventInviteBO> GetUserInvites(Guid userId);

        int JoinEvent(int eventFriendId, Guid appUserId);

        UserEventInviteBO GetInvite(Guid eventId, Guid userId);

        int InviteFriend(Guid eventId, Guid userId, InviteStatus status);

        List<UserEventInviteBO> GetPendingInvites(Guid eventId);

        List<ApplicationUser> GetRecommendedFriends(Guid userId, Guid currentEventId);
    }

    internal class EventInviteRepository : RepositoryBase, IEventInviteRepository
    {

        public EventInviteRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }
        public List<EventInviteBO> GetUserInvites(Guid userId)
        {
            var result = Connection.Query<EventInviteBO>($@"
                       select 
	                        inviteCore.Id as Id,
                            inviteCore.Name as EventName,
	                        u.NickName as EventOwner,
                            inviteCore.EventFriendId
                      from
                      (select
							e.Id,
                            e.Name,
                            e.UserId eventOwnerId,
                            ef.EventFriendId 
                        from
                            ApplicationUser u
                            inner join [dbo].[EventFriend] ef on u.Email = ef.Email
                            inner join[dbo].[Event] e on ef.Id = e.Id

                        where
                            u.Id  =  @{nameof(userId)}
                            and ef.Status in (2,3,4))inviteCore
				        inner join ApplicationUser u on u.Id = inviteCore.eventOwnerId",

                new { userId }, Transaction);

            return result.ToList();
        }

        public int JoinEvent(int eventFriendId, Guid appUserId)
        {
            var rows = Connection.Execute($@"UPDATE [EventFriend]
                    SET
                    [AppUserid] = @{nameof(appUserId)},
                    [Status] = 5
                    
                    WHERE [EventFriendId] = @{nameof(eventFriendId)}", new { appUserId, eventFriendId },transaction:Transaction);


            return rows;
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
	                    distinct
	                    u.Id,
	                    u.NickName,
	                    u.Email,
	                    u.AvatarId
                    from
	                    [dbo].[EventFriend] ef
	                    inner join ApplicationUser u on u.Id = ef.UserId
                    where
	                    ef.EventId <> @{nameof(currentEventId)} 
                        and ef.UserId = @{nameof(userId)}
                    ",

                new { currentEventId, userId }, Transaction);

            return result.ToList();
        }
    }
}
