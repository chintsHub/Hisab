using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Hisab.Common.BO;

namespace Hisab.Dapper.Repository
{
    public interface IEventTransactionRepository
    {
        

        List<TransactionBo> GetAllTransactions(int eventId);

        int CreateTransaction(NewTransactionBO newTransactionBO);

        int CreateTransactionSplit(List<TransactionSplitBO> splits);

        int CreateEventFriendJournals(List<EventFriendJournalBO> journals);

        int CreateEventTransactionJournal(EventTransactionJournalBO journal);

        int CreateEventTransactionJournals(List<EventTransactionJournalBO> journals);

        int CreateTransactionJournal(int transactionId, string particulars, int accountId, decimal debitAmount,
            decimal creditAmount);

        int CreateTransactionSettlement(int eventId, int transactionId, int payerEventFriendId,
            int receiverEventFriendId, decimal amount);

        decimal GetEventExpense(int eventId);

        decimal GetEventPoolBalance(int eventId);

        decimal GetEventFriendContributionAmount(int eventId, int eventFriendId);

        decimal GetEventFriendExpense(int eventId, int eventFriendId);

        List<SettlementData> GetSettlementData(int eventId);

        List<EventUserAccountBO> GetEventUserAccounts(Guid eventId);

        EventAccountBO GetEventAccount(Guid eventId);
    }

    internal class EventTransactionRepository : RepositoryBase, IEventTransactionRepository
    {
        public EventTransactionRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }

        public List<TransactionBo> GetAllTransactions(int eventId)
        {
           var result = Connection.Query<TransactionBo>($@"
                    SELECT 
	                       t.[Id]
                          ,[TotalAmount]
                          ,[Description]
                          ,[SplitType]
                          ,[Id]
                          ,[CreatedbyUserId]
                          ,[CreatedDateTime]
	                      ,u.NickName
                      FROM 
	                    [dbo].[EventTransaction] t
	                    inner join ApplicationUser u on t.CreatedbyUserId = u.Id
                        where t.Id = @{nameof(eventId)}",
                new { eventId }, Transaction);

            return result.ToList();
        }

        public List<SettlementData> GetSettlementData(int eventId)
        {
            var result = Connection.Query<SettlementData>($@"
                    select 
	                        s.Id,
	                        payer.NickName payerFriend,
	                        receiver.NickName receiverFriend,
	                        sum(Amount) Amount
                         FROM 
	                        [dbo].[EventTransactionSettlement] s
	                        inner join [dbo].[EventFriend] payer on payer.EventFriendId = s.PayerEventFriendId
	                        inner join [dbo].[EventFriend] receiver on receiver.EventFriendId = s.ReceiverEventFriendId
                        where 
                          s.Eventid = @{nameof(eventId)}
                        group by
	                        s.Id,
	                        payer.NickName ,
	                        receiver.NickName ",
                new { eventId }, Transaction);

            return result.ToList();
        }

        public decimal GetEventExpense(int eventId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select sum(TotalAmount) from [dbo].[EventTransaction] a where a.Id = @{nameof(eventId)}",
                     new { eventId }, Transaction);

            return totalAmount;
        }

        public decimal GetEventPoolBalance(int eventId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select
	                    sum(j.DebitAmount) -sum(j.CreditAmount) as balance
                    from
	                    [dbo].[EventTransactionJournal] j 
	                    inner join [dbo].[EventAccount] a on j.AccountId = a.AccountId
                    where
                        a.EventFriendId is null
	                    and a.Id = @{nameof(eventId)}
	                    and a.AccountTypeId = 1 -- current asset",
                new { eventId }, Transaction);

            return totalAmount;
        }

        public decimal GetEventFriendContributionAmount(int eventId, int eventFriendId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select
	                    sum(j.CreditAmount)
                    from
	                    [dbo].[EventTransactionJournal] j 
	                    inner join [dbo].[EventAccount] a on j.AccountId = a.AccountId
                    where
                        a.EventFriendId = @{nameof(eventFriendId)}
	                    and a.Id = @{nameof(eventId)}
	                    and a.AccountTypeId = 1 -- current asset",
                new { eventFriendId, eventId }, Transaction);

            return totalAmount;
        }

        public decimal GetEventFriendExpense(int eventId, int eventFriendId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                    SELECT 
	                    sum(s.AmountDue)
                     FROM 
	                    [dbo].[EventTransaction] t
	                    inner join [dbo].[EventTransactionSplit] s on t.Id = s.TransactionId
                    where
                        t.Id = @{nameof(eventId)}
	                    and s.EventFriendId = @{nameof(eventFriendId)}",
                new { eventId, eventFriendId }, Transaction);

            return totalAmount;
        }

        

        public int CreateTransaction(NewTransactionBO newTransactionBO)
        {
           

            string command = $@"

                    INSERT INTO [dbo].[EventTransaction]
                       ([Id]
                       ,[EventId]
                       ,[CreatedbyUserId]
                       ,[TransactionDate]
                       ,[TotalAmount]
                       ,[Description]
                       ,[PaidByUserId]
                       ,[LastModifiedDate]
                       ,[TransactionType]
                       ,[LendToFriendUserId])
                     VALUES
                           (@{nameof(newTransactionBO.TransactionId)}
                           ,@{nameof(newTransactionBO.EventId)}
                           ,@{nameof(newTransactionBO.CreatedByUserId)}
                           ,@{nameof(newTransactionBO.TransactionDate)}
                           ,@{nameof(newTransactionBO.TotalAmount)}
                           ,@{nameof(newTransactionBO.Description)}
                           ,@{nameof(newTransactionBO.PaidByUserId)},@{nameof(newTransactionBO.LastModifiedDate)}
                           ,@{nameof(newTransactionBO.TransactionType)},@{nameof(newTransactionBO.LendToFriendUserId)})   ";

            int transId = Connection.Execute(command,
                new
                {
                    newTransactionBO.TransactionId,
                    newTransactionBO.EventId,
                    newTransactionBO.CreatedByUserId,
                    newTransactionBO.TransactionDate,
                    newTransactionBO.TotalAmount,
                    newTransactionBO.Description,
                    newTransactionBO.PaidByUserId,
                    newTransactionBO.LastModifiedDate,
                    newTransactionBO.TransactionType,
                    newTransactionBO.LendToFriendUserId

                }, transaction: Transaction);


            return transId;
        }

        public int CreateTransactionSplit(List<TransactionSplitBO> splits)
        {
            int transIds = 0;

            foreach (var split in splits)
            {
                string command = $@"
                               INSERT INTO [dbo].[EventTransactionSplit]
                               ([EventId]
                               ,[UserId]
                               ,[TransactionId]
                               ,[SplitPercentage])
                             VALUES
                                   (@{nameof(split.EventId)}
                                   ,@{nameof(split.UserId)}
                                   ,@{nameof(split.TransactionId)}
                                   ,@{nameof(split.SplitPercentage)})      ";

                transIds += Connection.Execute(command,
                    new
                    {
                        split.EventId,
                        split.UserId,
                        split.TransactionId,
                        split.SplitPercentage

                    }, transaction: Transaction);
            }

           


            return transIds;
        }

        public int CreateTransactionJournal(int transactionId, string particulars, int accountId, decimal debitAmount, decimal creditAmount)
        {


            string command = $@"INSERT INTO [dbo].[EventTransactionJournal] ([TransactionId] ,[Particulars] ,[AccountId] ,[DebitAmount] ,[CreditAmount])
                    VALUES (@{nameof(transactionId)}, @{nameof(particulars)},@{nameof(accountId)},@{nameof(debitAmount)},@{nameof(creditAmount)} );
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    transactionId,
                    particulars,
                    accountId,
                    debitAmount,
                    creditAmount

                }, transaction: Transaction);


            return transId;
        }


        public int CreateTransactionSettlement(int eventId, int transactionId, int payerEventFriendId, int receiverEventFriendId, decimal amount)
        {


            string command = $@"INSERT INTO [dbo].[EventTransactionSettlement] ([Id] ,[TransactionId] ,[PayerEventFriendId] ,[ReceiverEventFriendId] ,[Amount])
                    VALUES (@{nameof(eventId)}, @{nameof(transactionId)},@{nameof(payerEventFriendId)}, @{nameof(receiverEventFriendId)}, @{nameof(amount)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    eventId,
                    transactionId,
                    payerEventFriendId,
                    receiverEventFriendId,
                    amount

                }, transaction: Transaction);


            return transId;
        }

        public List<EventUserAccountBO> GetEventUserAccounts(Guid eventId)
        {
            var result = Connection.Query<EventUserAccountBO>($@"
                   select
	                    f.EventId,
	                    f.UserId,
	                    ua.AccountId,
	                    ua.AccountTypeId
                    from
	                    [dbo].[ApplicationUser] u
	                    inner join [dbo].[EventFriend] f on u.Id = f.UserId
	                    inner join [dbo].[UserAccount] ua on ua.UserId = u.Id
                        
                    where f.EventId = @{nameof(eventId)}",
               new { eventId }, Transaction);

            return result.ToList();
        }

        public int CreateEventFriendJournals(List<EventFriendJournalBO> journals)
        {
            int transIds = 0;

            foreach (var journal in journals)
            {
                if(journal.PayRecieveFriendId == Guid.Empty)
                {
                    journal.PayRecieveFriendId = null;
                }
                
                string command = $@"
                             INSERT INTO [dbo].[EventFriendJournal]
                               ([EventId]
                               ,[UserId]
                               ,[TransactionId]
                               ,[DebitAccount]
                               ,[CreditAccount]
                               ,[PayReceiveFriend]
                               ,[Amount])
                             VALUES
                                   (@{nameof(journal.EventId)}
                                   ,@{nameof(journal.UserId)}
                                   ,@{nameof(journal.TransactionId)}
                                   ,@{nameof(journal.UserDebitAccountId)}
                                   ,@{nameof(journal.UserCreditAccountId)}
                                   ,@{nameof(journal.PayRecieveFriendId)}
                                   ,@{nameof(journal.Amount)})      ";

                transIds += Connection.Execute(command,
                    new
                    {
                        journal.EventId,
                        journal.UserId,
                        journal.TransactionId,
                        journal.UserDebitAccountId,
                        journal.UserCreditAccountId,
                        journal.PayRecieveFriendId,
                        journal.Amount

                    }, transaction: Transaction);
            }




            return transIds;
        }

        public EventAccountBO GetEventAccount(Guid eventId)
        {
            var result = Connection.Query<EventAccountBO>($@"
                              SELECT 
	                             [AccountId]
                                 ,[EventId]
                                 ,[AccountTypeId]
	                             ,COALESCE(DebitBalance.DebitTotal,0) DebitTotal
	                             ,COALESCE(CreditBalance.CreditTotal,0) CreditTotal
                            FROM [dbo].[EventAccount] e
                            left outer join
                            (select 
	                            EventAccountId,
	                            sum(Amount) DebitTotal
                             from [dbo].[EventTransactionJournal] e
                            where 
	                            e.EventId = @{nameof(eventId)} and e.EventAccountAction = 1 -- Debit balance
                            group by 
	                            EventAccountId) DebitBalance on e.AccountId = DebitBalance.EventAccountId
                            left outer join

                            (select 
	                            EventAccountId,
	                            sum(Amount) CreditTotal
                             from [dbo].[EventTransactionJournal] e
                            where 
	                            e.EventId = @{nameof(eventId)} and e.EventAccountAction = 2 -- Credit balance
                            group by 
	                            EventAccountId) CreditBalance on e.AccountId = CreditBalance.EventAccountId
                        
                            where e.EventId = @{nameof(eventId)}",
              new { eventId }, Transaction);

            return result.FirstOrDefault();
        }

        public int CreateEventTransactionJournal(EventTransactionJournalBO journal)
        {
            int transIds = 0;
            string command = $@"
                             INSERT INTO [dbo].[EventTransactionJournal]
                              ([EventId]
                               ,[UserId]
                               ,[TransactionId]
                               ,[EventAccountId]
                               ,[EventAccountAction]
                               ,[UserAccountId]
                               ,[EventFriendAccountAction]
                               ,[Amount])
                             VALUES
                                   (@{nameof(journal.EventId)}
                                   ,@{nameof(journal.UserId)}
                                   ,@{nameof(journal.TransactionId)}
                                   ,@{nameof(journal.EventAccountId)}
                                   ,@{nameof(journal.EventAccountAction)}
                                   ,@{nameof(journal.EventFriendAccountId)}
                                   ,@{nameof(journal.EventFriendAccountAction)}
                                   ,@{nameof(journal.Amount)})      ";

            transIds = Connection.Execute(command,
                new
                {
                    journal.EventId,
                    journal.UserId,
                    journal.TransactionId,
                    journal.EventAccountId,
                    journal.EventAccountAction,
                    journal.EventFriendAccountId,
                    journal.EventFriendAccountAction,
                    journal.Amount

                }, transaction: Transaction);

            return transIds;
        }

        public int CreateEventTransactionJournals(List<EventTransactionJournalBO> journals)
        {
            int retVal = 0;
            foreach(var journal in journals)
            {
                retVal += CreateEventTransactionJournal(journal);
            }

            return retVal;
        }
    }
}
