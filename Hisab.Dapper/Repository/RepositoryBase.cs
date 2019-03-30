using System;
using System.Data;

namespace Hisab.Dapper.Repository
{
    internal abstract class RepositoryBase
    {
        protected IDbTransaction Transaction { get; private set; }

        protected IDbConnection Connection => Transaction.Connection;

        public RepositoryBase(IDbTransaction transaction)
        {
            Transaction = transaction;
        }
    }
    
}
