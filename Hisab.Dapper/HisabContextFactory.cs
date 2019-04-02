using System.Data;
using System.Threading.Tasks;

namespace Hisab.Dapper
{
    public class HisabContextFactory
    {
        public static async Task<IHisabDbContext> InitializeUnitOfWorkAsync(IDbConnectionProvider connectionProvider)
        {
            var transaction =  await InitializeWithTransaction(connectionProvider);

            IHisabDbContext hisabContext = new HisabDbContext(transaction.Connection,transaction);

            return hisabContext;
        }

        public static async Task<IHisabDbContext> InitializeAsync(IDbConnectionProvider connectionProvider)
        {
            var connection = await Initialize(connectionProvider);

            IHisabDbContext hisabContext = new HisabDbContext(connection,null);

            return hisabContext;
        }

        private static async Task<IDbTransaction> InitializeWithTransaction(IDbConnectionProvider connectionProvider)
        {
            var connection = await connectionProvider.CreateConnectionAsync();

            var transaction = connection.BeginTransaction();
            return transaction;
        }

        private static async Task<IDbConnection> Initialize(IDbConnectionProvider connectionProvider)
        {
            var connection = await connectionProvider.CreateConnectionAsync();
            return connection;
        }
    }


}
