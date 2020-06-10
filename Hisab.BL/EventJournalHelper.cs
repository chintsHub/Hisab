using Hisab.Common.BO;
using Hisab.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hisab.BL
{
    public interface IEventJournalHelper
    {
        Task<List<EventFriendJournalBO>> CreateExpenseJournalPaidByFriend(NewTransactionBO newTransactionBO);

        Task<EventTransactionJournalBO> CreateContributeToPoolJournals(NewTransactionBO newTransactionBO);

        Task<List<EventFriendJournalBO>> CreateLendToFriendJournals(NewTransactionBO newTransactionBO);

        Task<List<EventTransactionJournalBO>> CreateExpenseJournalPaidFromPool(NewTransactionBO newTransactionBO);

        Task<EventUserAccountBO> GetExpenseAccount(Guid userId, Guid eventId);

        Task<EventUserAccountBO> GetCashAccount(Guid userId, Guid eventId);

        EventUserAccountBO GetAccountReceivable(Guid userId, Guid eventId);


    }
    
    public class EventJournalHelper : IEventJournalHelper
    {
        private IDbConnectionProvider _connectionProvider;

        public EventJournalHelper(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<EventTransactionJournalBO> CreateContributeToPoolJournals(NewTransactionBO newTransactionBO)
        {
            
            
            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var accounts = context.EventTransactionRepository.GetEventUserAccounts(newTransactionBO.EventId);

                var journal = new EventTransactionJournalBO()
                {
                    EventId = newTransactionBO.EventId,
                    UserId = newTransactionBO.PaidByUserId,
                    TransactionId = newTransactionBO.TransactionId,
                    EventAccountId = newTransactionBO.EventPoolAccountId,
                    EventAccountAction = JournalAction.Debit,
                    EventFriendAccountId = accounts.FirstOrDefault(x => x.UserId == newTransactionBO.PaidByUserId && x.AccountTypeId == ApplicationAccountType.Cash).AccountId,
                    EventFriendAccountAction = JournalAction.Credit,
                    Amount = newTransactionBO.TotalAmount
                };

                return journal;
            }


           
        }

        public async Task<List<EventTransactionJournalBO>> CreateExpenseJournalPaidFromPool(NewTransactionBO newTransactionBO)
        {
            
            var eventJournals = new List<EventTransactionJournalBO>();

            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var accounts = context.EventTransactionRepository.GetEventUserAccounts(newTransactionBO.EventId);

                foreach (var split in newTransactionBO.TransactionSplits)
                {

                    // In the books of the Event
                    eventJournals.Add(new EventTransactionJournalBO()
                        {
                            EventId = newTransactionBO.EventId,
                            UserId = split.UserId,
                            TransactionId = split.TransactionId,
                            // Event Friend Expense Account Debit and Event Cash Account Credit
                            EventAccountId = newTransactionBO.EventPoolAccountId,
                            EventAccountAction = JournalAction.Credit,
                            EventFriendAccountId = accounts.FirstOrDefault(x => x.UserId == split.UserId && x.AccountTypeId == ApplicationAccountType.Expense).AccountId,
                            EventFriendAccountAction = JournalAction.Debit,
                            Amount = split.SplitAmount,
                           
                        });

                    
                    
                }
            }


            return eventJournals;

        }

        public async Task<List<EventFriendJournalBO>> CreateExpenseJournalPaidByFriend(NewTransactionBO newTransactionBO)
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

        public async Task<List<EventFriendJournalBO>> CreateLendToFriendJournals(NewTransactionBO newTransactionBO)
        {
            var journals = new List<EventFriendJournalBO>();

            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                var accounts = context.EventTransactionRepository.GetEventUserAccounts(newTransactionBO.EventId);

                // This user has paid for a friend

                // In the books of the payer
                journals.Add(new EventFriendJournalBO()
                {
                    EventId = newTransactionBO.EventId,
                    // Debit Account Receivable Account and Credit Cash
                    UserDebitAccountId = accounts.FirstOrDefault(x => x.UserId == newTransactionBO.PaidByUserId && x.AccountTypeId == ApplicationAccountType.AccountRecievable).AccountId,
                    UserCreditAccountId = accounts.FirstOrDefault(x => x.UserId == newTransactionBO.PaidByUserId && x.AccountTypeId == ApplicationAccountType.Cash).AccountId,
                    Amount = newTransactionBO.TotalAmount,
                    TransactionId = newTransactionBO.TransactionId,
                    UserId = newTransactionBO.PaidByUserId,
                    PayRecieveFriendId = newTransactionBO.LendToFriendUserId
                });

                // In the books of the friend
                journals.Add(new EventFriendJournalBO()
                {
                    EventId = newTransactionBO.EventId,
                    // Debit Cash Account and credit Accounts payable
                    UserDebitAccountId = accounts.FirstOrDefault(x => x.UserId == newTransactionBO.LendToFriendUserId && x.AccountTypeId == ApplicationAccountType.Cash).AccountId,
                    UserCreditAccountId = accounts.FirstOrDefault(x => x.UserId == newTransactionBO.LendToFriendUserId && x.AccountTypeId == ApplicationAccountType.AccountPayable).AccountId,
                    Amount = newTransactionBO.TotalAmount,
                    TransactionId = newTransactionBO.TransactionId,
                    UserId = newTransactionBO.LendToFriendUserId.Value,
                    PayRecieveFriendId = newTransactionBO.PaidByUserId
                });

                return journals;
            }
        }

        public async Task<EventUserAccountBO> GetExpenseAccount(Guid userId, Guid eventId)
        {
            var retVal = new EventUserAccountBO();
            retVal.AccountTypeId = ApplicationAccountType.Expense;
            retVal.UserId = userId;
            retVal.EventId = eventId;
            retVal.CreditTotal = 0; // we never credit expense account in this app
            

            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                retVal.DebitTotal = 0;
               //expense paid from money pool
               var balanceFromEvents = context.EventTransactionRepository.GetUserAccountBalancesFromEventTransactions(eventId, userId, ApplicationAccountType.Expense);

                retVal.AccountTypeId = ApplicationAccountType.Expense;
                var DebitEntry = balanceFromEvents.Where(x => x.EventFriendAccountAction == JournalAction.Debit).FirstOrDefault();
                if(DebitEntry != null)
                {
                    retVal.DebitTotal += DebitEntry.TotalAmount;
                }
                

                //expense paid by friend
                var balanceFromEventFriend = context.EventTransactionRepository.GetDebitUserAccountBalanceFromEventFriendJournal(eventId, userId, ApplicationAccountType.Expense);
                if (balanceFromEventFriend != null)
                    retVal.DebitTotal += balanceFromEventFriend.TotalAmount;
            }

            return retVal;
        }

        public async Task<EventUserAccountBO> GetCashAccount(Guid userId, Guid eventId)
        {
            var retVal = new EventUserAccountBO();
            retVal.AccountTypeId = ApplicationAccountType.Cash;
            retVal.UserId = userId;
            retVal.EventId = eventId;
            


            using (var context = await HisabContextFactory.InitializeAsync(_connectionProvider))
            {
                retVal.CreditTotal = 0;
                retVal.DebitTotal = 0;

                //from Event books 
                var balanceFromEvents = context.EventTransactionRepository.GetUserAccountBalancesFromEventTransactions(eventId, userId, ApplicationAccountType.Cash);

                retVal.AccountTypeId = ApplicationAccountType.Cash;
                var DebitEntry = balanceFromEvents.Where(x => x.EventFriendAccountAction == JournalAction.Debit).FirstOrDefault();
                if (DebitEntry != null)
                {
                    retVal.DebitTotal += DebitEntry.TotalAmount;
                }

                var CreditEntry = balanceFromEvents.Where(x => x.EventFriendAccountAction == JournalAction.Credit).FirstOrDefault();
                if (CreditEntry != null)
                {
                    retVal.CreditTotal += CreditEntry.TotalAmount;
                }


                // from user books
                var creditbalanceFromUserBook = context.EventTransactionRepository.GetCreditUserAccountBalanceFromEventFriendJournal(eventId, userId, ApplicationAccountType.Cash);
                if (creditbalanceFromUserBook != null)
                {
                    retVal.CreditTotal += creditbalanceFromUserBook.TotalAmount;
                }
                var debitbalanceFromUserBook = context.EventTransactionRepository.GetDebitUserAccountBalanceFromEventFriendJournal(eventId, userId, ApplicationAccountType.Cash);
                if (debitbalanceFromUserBook != null)
                {
                    retVal.DebitTotal += debitbalanceFromUserBook.TotalAmount;
                }
            }

            return retVal;
        }

        public EventUserAccountBO GetAccountReceivable(Guid userId, Guid eventId)
        {
            throw new NotImplementedException();
        }
    }
}
