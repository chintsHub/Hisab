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
        public async void NotReturnNullForAccountTypes()
        {
            IDbConnectionProvider connection = new DbConnectionProvider(@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab2");

            IList<AccountType> accountTypes = null;
            using (var uow = new HisabDbContext(connection))
            {
                await uow.InitializeWithTransaction();
                accountTypes = uow.AccountTypeRepository.GetAccountTypes();
                
            }
            
            Assert.NotNull(accountTypes);


        }
        [Fact]
        public void ReturnTwoAccountTypes()
        {
            IList<AccountType> accountTypes = null;
            IDbConnectionProvider connection = new DbConnectionProvider(@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab2");
            using (var uow = new HisabDbContext(connection))
            {
                accountTypes = uow.AccountTypeRepository.GetAccountTypes();

            }


            Assert.Equal(2, accountTypes.Count);
        }
       
    }
}
