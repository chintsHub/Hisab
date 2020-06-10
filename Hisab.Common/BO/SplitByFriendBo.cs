using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    
    public class TransactionBO
    {
        public Guid EventId { get; set; }

        
        public Guid TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string TransactionDescription { get; set; }

        public string PaidByName { get; set; }
        public Guid PaidById { get; set; }
        public string PaidByEmail { get; set; }

        public TransactionType TransactionType { get; set; }

        public List<EventFriendBO> SharedWith { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid LendToFriendUserId { get; set; }
        public string LendToFriendName { get; set; }
    }
   


    public class SettlementBo
    {
        public int TransactionId { get; set; }

        public int EventId { get; set; }

        public int PayerFriendId { get; set; }

        public int ReceiverFriendId { get; set; }

        public decimal Amount { get; set; }
    }

    public class SettlementData
    {
        public int EventId { get; set; }

        public string PayerFriend { get; set; }

        public string ReceiverFriend { get; set; }

        public decimal Amount { get; set; }


    }

    public class TransactionJournalBo
    {
        public int TransactionId { get; set; }

        public string Particulars { get; set; }

        public int AccountId { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }
    }

    

    public class SplitPerFriendTransactionBo 
    {
        public List<SplitPerFriendBo> Friends { get; set; }

        public Decimal PaidByPoolAmount { get; set; }
        

        public SplitPerFriendTransactionBo()
        {
            Friends = new List<SplitPerFriendBo>();

            
        }
    }

    public class SplitPerFriendBo
    {
        public int EventFriendId { get; set; }

        public  decimal AmountDue { get; set; }

        public decimal AmountPaid { get; set; }

        public bool IncludeInSplit { get; set; }

        public decimal NetAmountToSettle { get; set; }

        public decimal AlreadySettled { get; set; }
    }

    public class EventPoolTransactionBo 
    {
        public int EventFriendId { get; set; }

        public EventPoolTransactionBo()
        {
            
        }
    }

    public class PoolFriendEntryBo
    {
        public int EventFriendId { get; set; }

        public decimal DepositAmount { get; set; }
    }

    public enum SplitType
    {
        NotApplicable = 0,
        SplitPerFriend = 1,
        SplitPerHead,
        SplitByRate
        
    }

    public class NewTransactionBO
    {
        public Guid TransactionId { get; set; }

        public Guid EventId { get; set; }

        public Guid EventPoolAccountId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Description { get; set; }

        public Guid PaidByUserId { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public List<TransactionSplitBO> TransactionSplits { get; set; }

        public TransactionType TransactionType { get; set; }

        public Guid? LendToFriendUserId { get; set; }

        public NewTransactionBO()
        {
            TransactionSplits = new List<TransactionSplitBO>();
        }
    }

    public class TransactionSplitBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid TransactionId { get; set; }

        public decimal SplitPercentage { get; set; }

        public decimal SplitAmount { get; set; }
    }

    public class EventFriendJournalBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid TransactionId { get; set; }

        public Guid UserDebitAccountId { get; set; }

        public Guid UserCreditAccountId { get; set; }

        public Guid? PayRecieveFriendId { get; set; }

        public Decimal Amount { get; set; }

    }

    public class EventTransactionJournalBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid TransactionId { get; set; }

        public Guid EventAccountId { get; set; }

        public JournalAction EventAccountAction { get; set; }

        public Guid EventFriendAccountId { get; set; }
        public JournalAction EventFriendAccountAction { get; set; }

        public decimal Amount { get; set; }

    }

    

    
    public enum TransactionType
    {
        Expense = 1,
        ContributionToPool = 2,
        LendToFriend = 3,
        ExpenseFromPool = 4,
        Settlement = 5
    }

    public enum JournalAction
    {
        Debit = 1,
        Credit = 2
    }
   
}
