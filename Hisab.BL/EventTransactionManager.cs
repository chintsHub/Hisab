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
        Task<bool> CreateExpenseTransaction(NewTransactionBO newTransactionBO);

        Task<EventAccountBO> GetEventAccount(Guid eventId);
    }

    public class EventTransactionManager : IEventTransactionManager
    {
        private IDbConnectionProvider _connectionProvider;

        public EventTransactionManager(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;

        }
        public async Task<bool> CreateExpenseTransaction(NewTransactionBO newTransactionBO)
        {
            var retVal = false;

            // setup
            newTransactionBO.TotalAmount = decimal.Round(newTransactionBO.TotalAmount, 2);
            newTransactionBO.LastModifiedDate = DateTime.UtcNow;
            newTransactionBO.TransactionType = TransactionType.Expense;
            SetTransactionId(newTransactionBO);

            // calculate split amount
            CalculateEqualSplitAndAmount(newTransactionBO.TransactionSplits, newTransactionBO.TotalAmount);

            //Event Friend Journals
            var eventFriendJournals = await CreateEventFriendJournals(newTransactionBO);

            using (var context = await HisabContextFactory.InitializeUnitOfWorkAsync(_connectionProvider))
            {


                var transRow = context.EventTransactionRepository.CreateTransaction(newTransactionBO);
                var splits = context.EventTransactionRepository.CreateTransactionSplit(newTransactionBO.TransactionSplits);

                var journals = context.EventTransactionRepository.CreateEventFriendJournals(eventFriendJournals);

                context.SaveChanges();
            }

            return retVal;
        }

        
        private async Task<List<EventFriendJournalBO>> CreateEventFriendJournals(NewTransactionBO newTransactionBO)
        {
            var journals = new List<EventFriendJournalBO>();

            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var accounts = context.EventTransactionRepository.GetEventUserAccounts(newTransactionBO.EventId);

                foreach (var split in newTransactionBO.TransactionSplits)
                {
                    if (split.UserId == newTransactionBO.PaidByUserId)
                    {
                        //This user has paid the bill
                        journals.Add(new EventFriendJournalBO() 
                        { 
                            EventId = newTransactionBO.EventId,
                            // Debit the expense and credit the cash
                            UserDebitAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.Expense).AccountId,
                            UserCreditAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.Cash).AccountId,
                            Amount = split.SplitAmount,
                            TransactionId = split.TransactionId,
                            UserId = split.UserId
                        });
                    }
                    else
                    {
                        // This user has paid for a friend

                        // In the books of the payer
                        journals.Add(new EventFriendJournalBO()
                        {
                            EventId = newTransactionBO.EventId,
                            // Debit Accounts receivable and Credit cash
                            UserDebitAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.AccountRecievable).AccountId,
                            UserCreditAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.Cash).AccountId,
                            Amount = split.SplitAmount,
                            TransactionId = split.TransactionId,
                            UserId = newTransactionBO.PaidByUserId,
                            PayRecieveFriendId = split.UserId
                        });

                        // In the books of the friend
                        journals.Add(new EventFriendJournalBO()
                        {
                            EventId = newTransactionBO.EventId,
                            // Debit expense and credit Accounts payable
                            UserDebitAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.Expense).AccountId,
                            UserCreditAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.AccountPayable).AccountId,
                            Amount = split.SplitAmount,
                            TransactionId = split.TransactionId,
                            UserId = split.UserId,
                            PayRecieveFriendId = newTransactionBO.PaidByUserId
                        });
                    }
                }
            }

            return journals;

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
    }
}
