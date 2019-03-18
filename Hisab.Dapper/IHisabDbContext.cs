using System;
using System.Threading.Tasks;
using Hisab.Dapper.Repository;

namespace Hisab.Dapper
{
    public interface IHisabDbContext : IDisposable
    {
        Task Initialize();

        IAccountTypeRepository AccountTypeRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }

        void SaveChanges();
    }
}
