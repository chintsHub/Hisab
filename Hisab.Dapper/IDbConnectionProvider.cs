using System.Data;
using System.Threading.Tasks;

namespace Hisab.Dapper
{
    public interface IDbConnectionProvider
    {
        Task<IDbConnection> CreateConnectionAsync();

        //IHisabDbContext GetContext();

        string GetConnectionString();
    }
}
