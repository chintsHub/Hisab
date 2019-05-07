using System;
using System.Collections.Generic;
using System.Text;
using Hisab.Dapper;
using Xunit;

namespace Hisab.Database.Test
{
    [CollectionDefinition("IntegrationTest collection")]
    public class IntegrationTestSetupFixtureCollection : ICollectionFixture<IntegrationTestSetupFixture>
    {
        //Empty wrapper class for IntegrationTestSetupFixture to use this for multiple Test files
    }

    public class IntegrationTestSetupFixture : IDisposable
    {
        public IDbConnectionProvider Connection { get; private set; }

        public IntegrationTestSetupFixture()
        {
            Connection = new DbConnectionProvider(@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab3");
        }

        public void Dispose()
        {
            Connection = null;
        }
    }
}
