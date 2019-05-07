using System.Data;
using System.Threading.Tasks;

namespace Hisab.Dapper
{
    public interface IDbConnectionProvider
    {
       
        //IHisabDbContext GetContext();

        string GetConnectionString();
    }
}
