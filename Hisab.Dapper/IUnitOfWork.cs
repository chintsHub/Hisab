using System;
using System.Data;
using System.Data.SqlClient;
using Hisab.Dapper.Repository;

namespace Hisab.Dapper
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountTypeRepository AccountTypeRepository { get; }

        void SaveChanges();
    }

    public class UnitofWork : IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IAccountTypeRepository _accountTypeRepository;
        private bool _disposed;

        public UnitofWork()
        {
            _connection = new SqlConnection(@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab2");
            _connection.Open();
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

        ~UnitofWork()
        {
            dispose(false);
        }

        public void Dispose()
        {
            dispose(false);
        }
    }
}
