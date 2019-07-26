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

    }

    internal class EventTransactionRepository : RepositoryBase, IEventTransactionRepository
    {
        public EventTransactionRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

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

        public int CreateTransaction(decimal totalAmount,int eventId, string description, int transactionType)
        {
           

            string command = $@"INSERT INTO [dbo].[EventTransaction] ([TotalAmount] ,[Description] ,[SplitType] ,[EventId])
                    VALUES (@{nameof(totalAmount)}, @{nameof(description)},@{nameof(transactionType)}, @{nameof(eventId)});
            SELECT CAST(SCOPE_IDENTITY() as int)";

            int transId = Connection.QuerySingle<int>(command,
                new
                {
                    totalAmount,
                    description,
                    transactionType,
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
