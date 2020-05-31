using Hisab.Common.BO;
using Hisab.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hisab.BL
{
    
    public interface IEventTransactionManager
    {
        Task<ManagerResponse> CreateExpenseTransaction(NewTransactionBO newTransactionBO);

        Task<bool> CreateContributeToPoolTransaction(NewTransactionBO newTransactionBO);

        Task<bool> CreateContributeToFriend(NewTransactionBO newTransactionBO);

        Task<EventAccountBO> GetEventAccount(Guid eventId);
    }

    public class EventTransactionManager : IEventTransactionManager
    {
        private IDbConnectionProvider _connectionProvider;
        private IEventJournalHelper _eventJournalHelper;

        public EventTransactionManager(IDbConnectionProvider connectionProvider, IEventJournalHelper eventJournalHelper)
        {
            _connectionProvider = connectionProvider;
            _eventJournalHelper = eventJournalHelper;

        }
        public async Task<ManagerResponse> CreateExpenseTransaction(NewTransactionBO newTransactionBO)
        {
            var retVal = new ManagerResponse();

            // setup
            newTransactionBO.TotalAmount = decimal.Round(newTransactionBO.TotalAmount, 2);
            newTransactionBO.LastModifiedDate = DateTime.UtcNow;
            
            SetTransactionId(newTransactionBO);

            // calculate split amount
            CalculateEqualSplitAndAmount(newTransactionBO.TransactionSplits, newTransactionBO.TotalAmount);

            //Journals
            List<EventFriendJournalBO> eventFriendJournals = null;
            List<EventTransactionJournalBO> transactionJournals = null;
            var eventAccount = await GetEventAccount(newTransactionBO.EventId);
            if (newTransactionBO.PaidByUserId == eventAccount.AccountId)
            {
                // Bill is paid from the pool amount
                if(eventAccount.CalculateBalance() >= newTransactionBO.TotalAmount)
                {
                    newTransactionBO.EventPoolAccountId = eventAccount.AccountId;
                    //overwriting as the exp is paid from pool. So setting paidByUserId is irrelevant
                    newTransactionBO.PaidByUserId = newTransactionBO.CreatedByUserId;
                    newTransactionBO.TransactionType = TransactionType.ExpenseFromPool;

                    transactionJournals = await _eventJournalHelper.CreateExpenseJournalPaidFromPool(newTransactionBO);
                 
                }
                else
                {
                    retVal.Messge = "Not enough balance in the Account to pay this bill";
                    retVal.Success = false;
                    return retVal;
                }
            }
            else
            {
                // Bill is paid by a friend
                newTransactionBO.TransactionType = TransactionType.Expense;
                eventFriendJournals = await _eventJournalHelper.CreateExpenseJournalPaidByFriend(newTransactionBO);
            }
            

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {


                var transRow = context.EventTransactionRepository.CreateTransaction(newTransactionBO);
                var splits = context.EventTransactionRepository.CreateTransactionSplit(newTransactionBO.TransactionSplits);
                               

                if(transactionJournals != null)
                {
                    context.EventTransactionRepository.CreateEventTransactionJournals(transactionJournals);
                }
                else
                {
                    var journals = context.EventTransactionRepository.CreateEventFriendJournals(eventFriendJournals);
                }
                

                context.SaveChanges();
            }

            return retVal;
        }

        
        

        private void SetTransactionId(NewTransactionBO newTransactionBO)
        {
            newTransactionBO.TransactionId = Guid.NewGuid();

            foreach(var split in newTransactionBO.TransactionSplits)
            {
                split.TransactionId = newTransactionBO.TransactionId;
            }
        }

        private void CalculateEqualSplitAndAmount(List<TransactionSplitBO> splits, decimal ExpenseAmount)
        {
            var perHead = decimal.Round((ExpenseAmount / splits.Count),2);
            var perHeadPercent = decimal.Round((100 / splits.Count),2);

            foreach(var split in splits)
            {
                split.SplitAmount = perHead;
                split.SplitPercentage = perHeadPercent;
            }

            var roundedExpenseAmount = perHead * splits.Count;
            if(roundedExpenseAmount > ExpenseAmount)
            {
                splits[0].SplitAmount = splits[0].SplitAmount - (roundedExpenseAmount - ExpenseAmount);
            }
            else
            {
                splits[0].SplitAmount = splits[0].SplitAmount + (ExpenseAmount - roundedExpenseAmount);
            }
        }

        public async Task<EventAccountBO> GetEventAccount(Guid eventId)
        {
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {

                var account = context.EventTransactionRepository.GetEventAccount(eventId);

                return account;

            }
        }

        public async Task<bool> CreateContributeToPoolTransaction(NewTransactionBO newTransactionBO)
        {

            var retVal = false;

            // setup
            newTransactionBO.TotalAmount = decimal.Round(newTransactionBO.TotalAmount, 2);
            newTransactionBO.LastModifiedDate = DateTime.UtcNow;
            newTransactionBO.TransactionType = TransactionType.ContributionToPool;
            newTransactionBO.TransactionId = Guid.NewGuid();

            //Event Journal
            var eventTransJournal = await _eventJournalHelper.CreateContributeToPoolJournals(newTransactionBO);

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {


                var transRow = context.EventTransactionRepository.CreateTransaction(newTransactionBO);
                

                var journals = context.EventTransactionRepository.CreateEventTransactionJournal(eventTransJournal);

                context.SaveChanges();
            }

            return retVal;
        }

        public async Task<bool> CreateContributeToFriend(NewTransactionBO newTransactionBO)
        {
            var retVal = false;

            // setup
            newTransactionBO.TotalAmount = decimal.Round(newTransactionBO.TotalAmount, 2);
            newTransactionBO.LastModifiedDate = DateTime.UtcNow;
            newTransactionBO.TransactionType = TransactionType.LendToFriend;
            newTransactionBO.TransactionId = Guid.NewGuid();

            //Event Journal
            var eventFriendJournals = await _eventJournalHelper.CreateLendToFriendJournals(newTransactionBO);

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {


                var transRow = context.EventTransactionRepository.CreateTransaction(newTransactionBO);


                var journals = context.EventTransactionRepository.CreateEventFriendJournals(eventFriendJournals);

                context.SaveChanges();
            }

            return retVal;
        }
    }
}
