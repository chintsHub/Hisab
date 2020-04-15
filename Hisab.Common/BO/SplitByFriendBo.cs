using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    public class TransactionBo
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        public  SplitType SplitType { get; set; }

        public List<TransactionJournalBo> Journals { get; set; }

        public List<SettlementBo> Settlements { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string NickName { get; set; }

        public DateTime CreatedDateTime { get; set; }


        public TransactionBo()
        {
            Settlements = new List<SettlementBo>();
        }
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

    public class EventAccountBo
    {
        public int AccountId { get; set; }

        public int EventId { get; set; }

        public int EventFriendId { get; set; }

        public int AccountTypeId { get; set; }
    }

    public class SplitPerFriendTransactionBo : TransactionBo
    {
        public List<SplitPerFriendBo> Friends { get; set; }

        public Decimal PaidByPoolAmount { get; set; }
        

        public SplitPerFriendTransactionBo()
        {
            Friends = new List<SplitPerFriendBo>();

            Journals = new List<TransactionJournalBo>();
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

    public class EventPoolTransactionBo : TransactionBo
    {
        public int EventFriendId { get; set; }

        public EventPoolTransactionBo()
        {
            Journals = new List<TransactionJournalBo>();
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

   
}
