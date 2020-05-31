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

    public class EventAccountBO : AccountBO
    {
        public Guid EventId { get; set; }

        public Guid AccountId { get; set; }

             
        

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
