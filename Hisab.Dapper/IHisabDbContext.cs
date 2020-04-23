using System;
using Hisab.Dapper.Repository;

namespace Hisab.Dapper
{
    public interface IHisabDbContext : IDisposable
    {
        IAccountTypeRepository AccountTypeRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }

        IApplicationRoleRepository ApplicationRoleRepository { get; }

        IEventRepository EventRepository { get; }

        IEventInviteRepository EventInviteRepository { get; }

        IEventTransactionRepository EventTransactionRepository { get; }

        IFeedbackRepository FeedbackRepository { get;  }

        void SaveChanges();
    } 


}
