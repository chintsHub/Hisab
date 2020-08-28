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
        

        List<TransactionBO> GetAllTransactions(Guid eventId);

        int CreateTransaction(NewTransactionBO newTransactionBO);

        int CreateTransactionSplit(List<TransactionSplitBO> splits);

        int CreateEventFriendJournals(List<EventFriendJournalBO> journals);

        int CreateEventTransactionJournal(EventTransactionJournalBO journal);

        int CreateEventTransactionJournals(List<EventTransactionJournalBO> journals);

        int CreateTransactionJournal(int transactionId, string particulars, int accountId, decimal debitAmount,
            decimal creditAmount);

        int UpdateComments(UpdateCommentsBO updateComment);

        int CreateTransactionSettlement(int eventId, int transactionId, int payerEventFriendId,
            int receiverEventFriendId, decimal amount);

        decimal GetEventExpense(Guid eventId);

        

        decimal GetEventFriendContributionAmount(int eventId, int eventFriendId);

        decimal GetEventFriendExpense(int eventId, int eventFriendId);

        List<SettlementData> GetSettlementData(int eventId);

        List<EventUserAccountBO> GetEventUserAccounts(Guid eventId);

       

        void DeleteTransaction(Guid transactionId);

        

        EventUserAccountRawBO GetDebitUserAccountBalanceFromEventFriendJournal(Guid eventId, Guid userId, ApplicationAccountType AccountType);

        EventUserAccountRawBO GetCreditUserAccountBalanceFromEventFriendJournal(Guid eventId, Guid userId, ApplicationAccountType AccountType);

        List<EventUserAccountRawBO> GetDebitBalanceForSettlementAccount(Guid eventId, Guid userId, ApplicationAccountType AccountType);

        List<EventUserAccountRawBO> GetCreditBalanceForSettlementAccount(Guid eventId, Guid userId, ApplicationAccountType AccountType);
    }

    internal class EventTransactionRepository : RepositoryBase, IEventTransactionRepository
    {
        public EventTransactionRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {

        }

        public List<TransactionBO> GetAllTransactions(Guid eventId)
        {
            var sql = $@"
                   SELECT 
	                    e.[Id] as TransactionId
                        ,e.[EventId]
                        ,e.Comments
                        
	                    ,[TransactionDate]
	                    ,[Description] as TransactionDescription
	                    ,[TransactionType]
                        ,[TotalAmount]

	                    ,[PaidByUserId] as PaidById
	                    ,u.NickName as PaidByName
                        ,u.Email as PaidByEmail
	     
                        ,[PaidToFriendUserId]
	                    ,u1.NickName PaidToFriendName

	                    ,s.UserId 
	                    ,u3.NickName 
                    FROM 
	                    EventTransaction e 
                        left outer join ApplicationUser u on u.Id = e.PaidByUserId
	                    left outer join ApplicationUser u1 on u1.Id = e.PaidToFriendUserId
	                    left outer join [EventTransactionSplit] s on s.TransactionId = e.Id
	                    left outer join ApplicationUser u3 on u3.Id = s.UserId
	                where
                        e.EventId = @{nameof(eventId)}";

            var eventDict = new Dictionary<Guid, TransactionBO>();

            var result = Connection.Query<TransactionBO, EventFriendBO, TransactionBO>(sql,
                (eventTransactionBO, eventFriend) =>
                {
                    TransactionBO transBo;

                    if (!eventDict.TryGetValue(eventTransactionBO.TransactionId, out transBo))
                    {
                        transBo = eventTransactionBO;
                        transBo.SharedWith = new List<EventFriendBO>();
                        transBo.TransactionId = eventTransactionBO.TransactionId;
                        
                        eventDict.Add(transBo.TransactionId, transBo);
                    }
                    transBo.SharedWith.Add(eventFriend);

                    return transBo;
                },
                new { eventId }, Transaction, splitOn: "UserId").ToList();



            return eventDict.Values.ToList();
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

        public decimal GetEventExpense(Guid eventId)
        {
            var totalAmount = Connection.ExecuteScalar<decimal>($@"
                     select sum(TotalAmount) from [dbo].[EventTransaction] a 
                        where 
                            a.TransactionType in (1,4) and
                            a.EventId = @{nameof(eventId)}",
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
                       ,[PaidToFriendUserId])
                     VALUES
                           (@{nameof(newTransactionBO.TransactionId)}
                           ,@{nameof(newTransactionBO.EventId)}
                           ,@{nameof(newTransactionBO.CreatedByUserId)}
                           ,@{nameof(newTransactionBO.TransactionDate)}
                           ,@{nameof(newTransactionBO.TotalAmount)}
                           ,@{nameof(newTransactionBO.Description)}
                           ,@{nameof(newTransactionBO.PaidByUserId)},@{nameof(newTransactionBO.LastModifiedDate)}
                           ,@{nameof(newTransactionBO.TransactionType)},@{nameof(newTransactionBO.PaidToFriendUserId)})   ";

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
                    newTransactionBO.PaidToFriendUserId

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

        public void DeleteTransaction(Guid transactionId)
        {

            var transactionJournalResult = Connection.Execute($@" delete  FROM [dbo].[EventTransactionJournal] where [TransactionId] = @{nameof(transactionId)}",
              new { transactionId }, Transaction);

            var eventFriendJournalResult = Connection.Execute($@" delete  FROM [dbo].[EventFriendJournal] where [TransactionId] = @{nameof(transactionId)}",
              new { transactionId }, Transaction);

            var eventsplitResult = Connection.Execute($@" delete  FROM [dbo].[EventTransactionSplit] where [TransactionId] = @{nameof(transactionId)}",
             new { transactionId }, Transaction);

            var transactionResult = Connection.Execute($@" delete  FROM [dbo].[EventTransaction] where [Id] = @{nameof(transactionId)}",
              new { transactionId }, Transaction);

            
        }

        

        public EventUserAccountRawBO GetDebitUserAccountBalanceFromEventFriendJournal(Guid eventId, Guid userId, ApplicationAccountType AccountType)
        {
            var EventTransactionJournalResult = Connection.Query<EventUserAccountRawBO>($@"
                    SELECT
	                   j.EventId,
	                   -- j.[DebitAccount] as AccountId,
	                   1 as EventFriendAccountAction,
	                   sum(j.Amount) TotalAmount -- Debit balance
                      FROM [EventFriendJournal] j
                        inner join [dbo].[UserAccount] ua on ua.AccountId = j.[DebitAccount]
	                    inner join [dbo].[ApplicationAccountType] a on a.Id = ua.AccountTypeId
                    where 
	                    j.EventId = @{nameof(eventId)}
	                    and j.UserId = @{nameof(userId)}
						and a.Id = @{nameof(AccountType)}
                    group by
	                    j.EventId
	                    -- j.DebitAccount ",
               new { eventId, userId, AccountType }, Transaction);

            return EventTransactionJournalResult.FirstOrDefault();
        }

        public EventUserAccountRawBO GetCreditUserAccountBalanceFromEventFriendJournal(Guid eventId, Guid userId, ApplicationAccountType AccountType)
        {
            var EventTransactionJournalResult = Connection.Query<EventUserAccountRawBO>($@"
                    SELECT
	                   j.EventId,
	                   -- j.[CreditAccount] as AccountId,
	                   2 as EventFriendAccountAction, --Credit
	                   sum(j.Amount) TotalAmount -- Credit balance
                      FROM [EventFriendJournal] j
                         inner join [dbo].[UserAccount] ua on ua.AccountId = j.[CreditAccount]
	                    inner join [dbo].[ApplicationAccountType] a on a.Id = ua.AccountTypeId
                    where 
	                    j.EventId = @{nameof(eventId)}
	                    and j.UserId = @{nameof(userId)}
						and a.Id = @{nameof(AccountType)}
                    group by
	                    j.EventId
	                    -- j.CreditAccount",
              new { eventId, userId, AccountType }, Transaction);

            return EventTransactionJournalResult.FirstOrDefault();
        }

        public List<EventUserAccountRawBO> GetDebitBalanceForSettlementAccount(Guid eventId, Guid userId, ApplicationAccountType AccountType)
        {
            if(AccountType == ApplicationAccountType.Cash || AccountType == ApplicationAccountType.Expense)
            {
                throw new Exception("Invalid Account Type for Settlement");
            }
            
            var EventTransactionJournalResult = Connection.Query<EventUserAccountRawBO>($@"
                   SELECT
	                   j.EventId,
	                   j.[DebitAccount] as AccountId,
	                   1 as EventFriendAccountAction,
					   j.[PayReceiveFriend],
					  
	                   sum(j.Amount) TotalAmount -- Debit balance
                      FROM [EventFriendJournal] j
                        inner join [dbo].[UserAccount] ua on ua.AccountId = j.[DebitAccount]
	                    inner join [dbo].[ApplicationAccountType] a on a.Id = ua.AccountTypeId
						inner join [dbo].[ApplicationUser] u on u.Id = j.[PayReceiveFriend]
                    where 
	                    j.EventId = @{nameof(eventId)} -- in the event
	                    and j.UserId = @{nameof(userId)} -- in the user book
						and a.Id = @{nameof(AccountType)} -- Accounts Type
                    group by
	                    j.EventId,
	                    j.DebitAccount ,
						[PayReceiveFriend]",
              new { eventId, userId, AccountType }, Transaction);

            return EventTransactionJournalResult.ToList();
        }

        public List<EventUserAccountRawBO> GetCreditBalanceForSettlementAccount(Guid eventId, Guid userId, ApplicationAccountType AccountType)
        {
            if (AccountType == ApplicationAccountType.Cash || AccountType == ApplicationAccountType.Expense)
            {
                throw new Exception("Invalid Account Type for Settlement");
            }

            var EventTransactionJournalResult = Connection.Query<EventUserAccountRawBO>($@"
                   SELECT
	                   j.EventId,
	                   j.[CreditAccount] as AccountId,
	                   1 as EventFriendAccountAction,
					   j.[PayReceiveFriend],
					  
	                   sum(j.Amount) TotalAmount -- Credit balance
                      FROM [EventFriendJournal] j
                        inner join [dbo].[UserAccount] ua on ua.AccountId = j.[CreditAccount]
	                    inner join [dbo].[ApplicationAccountType] a on a.Id = ua.AccountTypeId
						inner join [dbo].[ApplicationUser] u on u.Id = j.[PayReceiveFriend]
                    where 
	                    j.EventId = @{nameof(eventId)} -- in the event
	                    and j.UserId = @{nameof(userId)} -- in the user book
						and a.Id = @{nameof(AccountType)} -- Accounts Type
                    group by
	                    j.EventId,
	                    j.CreditAccount,
						[PayReceiveFriend]		 ",
              new { eventId, userId, AccountType }, Transaction);

            return EventTransactionJournalResult.ToList();
        }

        public int UpdateComments(UpdateCommentsBO updateComment)
        {
            string command = $@"

                   UPDATE [dbo].[EventTransaction]
                   SET 
	                    [LastModifiedDate] = @{nameof(updateComment.LastModifiedDate)}
                        ,[Comments] = @{nameof(updateComment.Comments)}
                    WHERE 
	                Id = @{nameof(updateComment.TransactionId)}
	                and EventId = @{nameof(updateComment.EventId)}     ";

            int row = Connection.Execute(command,
                new
                {
                    updateComment.TransactionId,
                    updateComment.EventId,
                    updateComment.Comments,
                    updateComment.LastModifiedDate
                   

                }, transaction: Transaction);


            return row;
        }
    }
}
