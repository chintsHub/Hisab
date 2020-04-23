
using System.Data;

namespace Hisab.Dapper.Repository
{
    internal abstract class RepositoryBase
    {
        protected IDbTransaction Transaction { get; private set; }

        private IDbConnection _connection;
        protected IDbConnection Connection
        {
            get
            {
                if (Transaction == null)
                    return _connection;

               return Transaction.Connection;
            }
        }

        public RepositoryBase(IDbConnection connection, IDbTransaction transaction)
        {
            Transaction = transaction;
            _connection = connection;
        }
    }
    
}
