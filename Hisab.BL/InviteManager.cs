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
        Task<List<EventInviteBO>> GetUserInvites(int userId);

        Task<int> JoinEvent(int eventFriendId, int appUserId);
    }

    public class EventInviteManager : IEventInviteManager
    {
        private IDbConnectionProvider _connectionProvider;

        public EventInviteManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<List<EventInviteBO>> GetUserInvites(int userId)
        {


            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.GetUserInvites(userId);

                return events;

            }
        }

        public async Task<int> JoinEvent(int eventFriendId, int appUserId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var events = context.EventInviteRepository.JoinEvent(eventFriendId, appUserId);

                return events;

            }
        }
    }
}
