using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hisab.Dapper
{
    public static class HisabContextFactory
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
            try
            {
                var sqlConnection = new SqlConnection(connectionProvider.GetConnectionString());
                await sqlConnection.OpenAsync();

                var transaction = sqlConnection.BeginTransaction();
                return transaction;

            }
            catch( Exception ex)
            {
                throw ex;
            }

            

           
        }

        private static async Task<IDbConnection> Initialize(IDbConnectionProvider connectionProvider)
        {
            try
            {
                var sqlConnection = new SqlConnection(connectionProvider.GetConnectionString());
                await sqlConnection.OpenAsync();

                
                return sqlConnection;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


}
