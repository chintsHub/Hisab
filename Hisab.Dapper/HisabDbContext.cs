﻿using System.Data;
using System.Threading.Tasks;
using Hisab.Dapper.Repository;


namespace Hisab.Dapper
{
    public class HisabDbContext : IHisabDbContext
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        


        private IAccountTypeRepository _accountTypeRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IApplicationRoleRepository _applicationRoleRepository;
        private IEventRepository _eventRepository;
        private IEventInviteRepository _eventInviteRepository;
        private IEventTransactionRepository _eventTransactionRepository;
        private IFeedbackRepository _feedbackRepository;
        private bool _disposed;
        

        public HisabDbContext(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
            //@"Integrated Security=SSPI;Pooling=false;Data Source=BullsEye\Chints;Initial Catalog=Hisab2");
            

        }

   
        public void CloseConnection()
        {
            if(_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public IAccountTypeRepository AccountTypeRepository
        {
            get
            {
                if (_accountTypeRepository == null)
                {
                    _accountTypeRepository = new AccountTypeRepository(_connection,_transaction);
                    
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
                    _applicationUserRepository = new ApplicationUserRepository(_connection, _transaction);

                }

                return _applicationUserRepository;
            }
        }

        public IApplicationRoleRepository ApplicationRoleRepository
        {
            get
            {
                if (_applicationRoleRepository == null)
                {
                    _applicationRoleRepository = new ApplicationRoleRepository(_connection, _transaction);
                }

                return _applicationRoleRepository;
            }


        }

        public IEventRepository EventRepository
        {
            get
            {
                if (_eventRepository == null)
                {
                    _eventRepository = new EventRepository(_connection, _transaction);
                }

                return _eventRepository;
            }


        }

        public IEventInviteRepository EventInviteRepository
        {
            get
            {
                if (_eventInviteRepository == null)
                {
                    _eventInviteRepository = new EventInviteRepository(_connection, _transaction);
                }

                return _eventInviteRepository;
            }


        }

        public IEventTransactionRepository EventTransactionRepository
        {
            get
            {
                if (_eventTransactionRepository == null)
                {
                    _eventTransactionRepository = new EventTransactionRepository(_connection, _transaction);
                }

                return _eventTransactionRepository;
            }


        }

        public IFeedbackRepository FeedbackRepository
        {
            get
            {
                if (_feedbackRepository == null)
                {
                    _feedbackRepository = new FeedbackRepository(_connection, _transaction);
                }

                return _feedbackRepository;
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
                _transaction?.Dispose();

                resetRepositories();
            }

            //SELECT * FROM sys.sysprocesses WHERE open_tran = 1
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
            dispose(true);
        }
    }
}
