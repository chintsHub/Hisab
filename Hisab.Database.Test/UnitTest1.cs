using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Hisab.Dapper;
using Hisab.Dapper.Model;
using Hisab.Dapper.Repository;
using Xunit;

namespace Hisab.Database.Test
{
    public class AccountRepositoryShould
    {
        [Fact]
        public void NotReturnNullForAccountTypes()
        {
            IList<AccountType> accountTypes = null;
            using (var uow = new UnitofWork())
            {
                accountTypes = uow.AccountTypeRepository.GetAccountTypes();
                
            }
            Assert.NotNull(accountTypes);


        }
        [Fact]
        public void ReturnTwoAccountTypes()
        {
            IList<AccountType> accountTypes = null;
            using (var uow = new UnitofWork())
            {
                accountTypes = uow.AccountTypeRepository.GetAccountTypes();

            }


            Assert.Equal(2, accountTypes.Count);
        }
       
    }
}
