using System.Data;
using System.Threading.Tasks;
using Hisab.Dapper.Repository;

namespace Hisab.Dapper
{
    public class HisabDbContext : IHisabDbContext
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IDbConnectionProvider _connectionProvider;


        private IAccountTypeRepository _accountTypeRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private bool _disposed;
        

        public HisabDbContext(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
                //@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab2");

            
        }

        public async  Task Initialize()
        {
            _connection = await _connectionProvider.CreateConnectionAsync();
         
            _transaction = _connection.BeginTransaction();
        }
        
      

        public IAccountTypeRepository AccountTypeRepository
        {
            get
            {
                if (_accountTypeRepository == null)
                {
                    _accountTypeRepository = new AccountTypeRepository(_transaction);
                    
                }

                return _accountTypeRepository;
            }
        }

        public IApplicationUserRepository ApplicationUserRepository
        {
            get
            {
                if (_applicationUserRepository == null)
                {
                    _applicationUserRepository = new ApplicationUserRepository(_transaction);

                }

                return _applicationUserRepository;
            }
        }

        public void SaveChanges()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                resetRepositories();
            }
        }

        private void resetRepositories()
        {
            _accountTypeRepository = null;
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~HisabDbContext()
        {
            dispose(false);
        }

        public void Dispose()
        {
            dispose(false);
        }
    }
}
