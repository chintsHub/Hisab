using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hisab.Common.BO;
using Hisab.Dapper;

namespace Hisab.BL
{
    public interface IEventInviteManager
    {
        Task<List<EventInviteBO>> GetUserInvites(Guid userId);

        Task<int> JoinEvent(int eventFriendId, Guid appUserId);
    }

    public class EventInviteManager : IEventInviteManager
    {
        private IDbConnectionProvider _connectionProvider;

        public EventInviteManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<List<EventInviteBO>> GetUserInvites(Guid userId)
        {


            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetUserInvites(userId);

                return events;

            }
        }

        public async Task<int> JoinEvent(int eventFriendId, Guid appUserId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.JoinEvent(eventFriendId, appUserId);

                return events;

            }
        }
    }
}
