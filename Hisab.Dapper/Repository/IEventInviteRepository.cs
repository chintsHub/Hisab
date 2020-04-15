using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Hisab.Common.BO;

namespace Hisab.Dapper.Repository
{
    public interface IEventInviteRepository
    {
        List<EventInviteBO> GetUserInvites(Guid userId);

        int JoinEvent(int eventFriendId, Guid appUserId);
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
	                        inviteCore.Id as EventId,
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
                            inner join[dbo].[Event] e on ef.EventId = e.Id

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
    }
}
