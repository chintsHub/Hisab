﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Hisab.Dapper.Model;
using Dapper;




namespace Hisab.Dapper.Repository
{
    public interface IAccountTypeRepository
    {
        IList<AccountType> GetAccountTypes();
    }

   
    internal class AccountTypeRepository : RepositoryBase, IAccountTypeRepository
    {
      

        public AccountTypeRepository(IDbTransaction transaction):base(transaction)
        {
            
            
        }
    

        public IList<AccountType> GetAccountTypes()
        {
            List<AccountType> retVal = new List<AccountType>();

            retVal =  Connection.Query<AccountType>("select * from AccountType",transaction:Transaction).ToList();

           
            
            return retVal;
        }
    }
}
