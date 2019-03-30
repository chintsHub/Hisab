using System;
using System.Threading.Tasks;
using Hisab.Dapper.Repository;

namespace Hisab.Dapper
{
    public interface IHisabDbContext : IDisposable
    {
        Task InitializeWithTransaction();
        

        IAccountTypeRepository AccountTypeRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }

        IApplicationRoleRepository ApplicationRoleRepository { get; }

        void SaveChanges();
    }
}
