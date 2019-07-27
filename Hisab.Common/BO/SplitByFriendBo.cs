using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    public abstract class TransactionBo
    {
        public int EventId { get; set; }

        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        public  SplitType SplitType { get; set; }

        public List<TransactionJournalBo> Journals { get; set; }

        //public abstract int ProcessTransaction();

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
