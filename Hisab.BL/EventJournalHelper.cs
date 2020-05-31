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
                            // Event Friend Account Debit and Event Cash Account Credit
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
    }
}
