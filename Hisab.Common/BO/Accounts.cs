using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    public class UserAccountBO
    {
        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }

        public ApplicationAccountType AccountType { get; set; }


    }

    public enum ApplicationAccountType
    {
        Cash = 1,
        Expense = 2,
        AccountRecievable = 3,
        AccountPayable = 4
    }

    public class EventUserAccountBO : AccountBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }


    }

    public class EventUserAccountRawBO
    {
        public Guid EventId { get; set; }

        public Guid UserAccountId { get; set; }

        public JournalAction EventFriendAccountAction { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid PayReceiveFriend { get; set; }

      

    }

    public class EventAccountBO : AccountBO
    {
        public Guid EventId { get; set; }

        public Guid AccountId { get; set; }

        public string EventName { get; set; }



    }

   

    public class SettlementAccountBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid FriendId { get; set; }

        public string FriendName { get; set; }

        public AvatarEnum FriendAvatar { get; set; }

        public AccountBO AccountPayable { get; set; }

        public AccountBO AccountReceivable { get; set; }

        public SettlementAccountBO()
        {
            AccountPayable = new AccountBO();
            AccountReceivable = new AccountBO();

            AccountPayable.AccountTypeId = ApplicationAccountType.AccountPayable;
            AccountReceivable.AccountTypeId = ApplicationAccountType.AccountRecievable;

        }
    }

    

    public class AccountBO
    {
        public ApplicationAccountType AccountTypeId { get; set; }

        public decimal DebitTotal { get; set; }

        public decimal CreditTotal { get; set; }

        public decimal CalculateBalance()
        {
            decimal balance=0;
            switch (AccountTypeId)
            {
                case ApplicationAccountType.Cash:
                    balance = DebitTotal - CreditTotal;
                    break;
                case ApplicationAccountType.Expense:
                    balance = DebitTotal - CreditTotal;
                    break;
                case ApplicationAccountType.AccountPayable:
                    balance = CreditTotal - DebitTotal;
                    break;
                case ApplicationAccountType.AccountRecievable:
                    balance = DebitTotal - CreditTotal;
                    break;

            }

            return balance;
        }
    }
}
