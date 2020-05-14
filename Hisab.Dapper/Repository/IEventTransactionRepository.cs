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
        List<EventAccountBo> GetAccountsForEvent(int eventId);

        List<TransactionBo> GetAllTransactions(int eventId);

        int CreateTransaction(decimal totalAmount, int eventId, string description, int transactionType, Guid createdByUserId, DateTime createDateTime);

        int CreateTransactionSplit(decimal amountDue, int transactionId, int eventFriendId);

        int CreateTransactionJournal(int transactionId, string particulars, int accountId, decimal debitAmount,
            decimal creditAmount);

        int CreateTransactionSettlement(int eventId, int transactionId, int payerEventFriendId,
            int receiverEventFriendId, decimal amount);

        decimal GetEventExpense(int eventId);

        decimal GetEventPoolBalance(int eventId);

        decimal GetEventFriendContributionAmount(int eventId, int eventFriendId);

        decimal GetEventFriendExpense(int eventId, int eventFriendId);

        List<SettlementData> GetSettlementData(int eventId);


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

        public List<EventAccountBo> GetAccountsForEvent(int eventId)
        {
            var result = Connection.Query<EventAccountBo>($@"
                    select 
	                    AccountId,
	                    Id,
	                    EventFriendId,
                        AccountTypeId
	               
                from 
	                [dbo].[EventAccount] a
	               
                where
                 a.Id = @{nameof(eventId)}",

                new { eventId }, Transaction);

            return result.ToList();
        }

        public int CreateTransaction(decimal totalAmount,int eventId, string description, int splitType, Guid createdByUserId, DateTime createdDateTime)
        {
           

            string command = $@"INSERT INTO [dbo].[EventTransaction] ([TotalAmount] ,[Description] ,[SplitType] ,[Id], [CreatedbyUserId] ,[CreatedDateTime])
                    VALUES (@{nameof(totalAmount)}, @{nameof(description)},@{nameof(splitType)}, @{nameof(eventId)}, @{nameof(createdByUserId)}, @{nameof(createdDateTime)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    totalAmount,
                    description,
                    splitType,
                    eventId,
                    createdByUserId,
                    createdDateTime

                }, transaction: Transaction);


            return transId;
        }

        public int CreateTransactionSplit(decimal amountDue, int transactionId, int eventFriendId)
        {


            string command = $@"INSERT INTO [dbo].[EventTransactionSplit] ([EventFriendId] ,[TransactionId] ,[AmountDue])
                    VALUES (@{nameof(eventFriendId)}, @{nameof(transactionId)},@{nameof(amountDue)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    eventFriendId,
                    transactionId,
                    amountDue
                 

                }, transaction: Transaction);


            return transId;
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
    }
}
