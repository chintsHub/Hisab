using System;
using System.Collections.Generic;
using Hisab.Dapper;
using Hisab.Dapper.Model;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Hisab.Database.Test
{
    [Collection("IntegrationTest collection")] // xunit will inject IntegrationTestSetupFixture and the object will be shared across all Test classes
    public class AccountTypeRepositoryShould : IDisposable //, IClassFixture<IntegrationTestSetupFixture> -: Use to share context between Tests during execution
    {
        private IDbConnectionProvider _connection;
        private IList<AccountType> _sut;
        private ILogger _output;

        public AccountTypeRepositoryShould(ITestOutputHelper outputHelper, IntegrationTestSetupFixture integrationTestSetupFixture)
        {
            _connection = integrationTestSetupFixture.Connection;


            _output = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(outputHelper, LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<AccountTypeRepositoryShould>();
        }

        [Fact]
        [Trait("Category","SeedingDataTest")]
        public async void NotReturnNullForAccountTypes()
        {
            

           _output.Information("Running Integration Test for AccountTypeId:NotReturnNullForAccountTypes ");
           

            using (var context = await HisabContextFactory.InitializeAsync(_connection))
            {
                _sut = context.AccountTypeRepository.GetAccountTypes();
            }

                Assert.NotNull(_sut);


        }
        [Fact]
        public async void ReturnTwoAccountTypes()
        {
            _output.Information("Running Integration Test for AccountTypeId:ReturnTwoAccountTypes ");
            using (var context = await HisabContextFactory.InitializeAsync(_connection))
            {
                _sut = context.AccountTypeRepository.GetAccountTypes();
            }


            Assert.Equal(2, _sut.Count);
        }

        public void Dispose()
        {
            _sut = null;
        }
    }
}
