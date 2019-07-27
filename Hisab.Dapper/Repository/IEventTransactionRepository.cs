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

        int CreateTransaction(decimal totalAmount, int eventId, string description, int transactionType);

        int CreateTransactionSplit(decimal amountDue, int transactionId, int eventFriendId);

        int CreateTransactionJournal(int transactionId, string particulars, int accountId, decimal debitAmount,
            decimal creditAmount);

        decimal GetEventExpense(int eventId);

        decimal GetEventPoolBalance(int eventId);

        decimal GetEventFriendContributionAmount(int eventId, int eventFriendId);

        decimal GetEventFriendExpense(int eventId, int eventFriendId);

    }

    internal class EventTransactionRepository : RepositoryBase, IEventTransactionRepository
    {
        public EventTransactionRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }

        public decimal GetEventExpense(int eventId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select sum(TotalAmount) from [dbo].[EventTransaction] a where a.EventId = @{nameof(eventId)}",
                     new { eventId }, Transaction);

            return totalAmount;
        }

        public decimal GetEventPoolBalance(int eventId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select
	                    sum(j.DebitAmount) -sum(j.CreditAmount) as balance
                    from
	                    [dbo].[EventTranscationJournal] j 
	                    inner join [dbo].[EventAccount] a on j.AccountId = a.AccountId
                    where
                        a.EventFriendId is null
	                    and a.EventId = @{nameof(eventId)}
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
	                    [dbo].[EventTranscationJournal] j 
	                    inner join [dbo].[EventAccount] a on j.AccountId = a.AccountId
                    where
                        a.EventFriendId = @{nameof(eventFriendId)}
	                    and a.EventId = @{nameof(eventId)}
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
                        t.EventId = @{nameof(eventId)}
	                    and s.EventFriendId = @{nameof(eventFriendId)}",
                new { eventId, eventFriendId }, Transaction);

            return totalAmount;
        }

        public List<EventAccountBo> GetAccountsForEvent(int eventId)
        {
            var result = Connection.Query<EventAccountBo>($@"
                    select 
	                    AccountId,
	                    EventId,
	                    EventFriendId,
                        AccountTypeId
	               
                from 
	                [dbo].[EventAccount] a
	               
                where
                 a.EventId = @{nameof(eventId)}",

                new { eventId }, Transaction);

            return result.ToList();
        }

        public int CreateTransaction(decimal totalAmount,int eventId, string description, int splitType)
        {
           

            string command = $@"INSERT INTO [dbo].[EventTransaction] ([TotalAmount] ,[Description] ,[SplitType] ,[EventId])
                    VALUES (@{nameof(totalAmount)}, @{nameof(description)},@{nameof(splitType)}, @{nameof(eventId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    totalAmount,
                    description,
                    splitType,
                    eventId

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


            string command = $@"INSERT INTO [dbo].[EventTranscationJournal] ([TransactionId] ,[Particulars] ,[AccountId] ,[DebitAmount] ,[CreditAmount])
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
    }
}
