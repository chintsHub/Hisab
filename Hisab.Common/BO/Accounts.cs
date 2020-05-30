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

    public class EventUserAccountBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }

        public ApplicationAccountType AccountTypeId { get; set; }
    }

    public class EventAccountBO
    {
        public Guid EventId { get; set; }

        public Guid AccountId { get; set; }

        public ApplicationAccountType AccountTypeId { get; set; }

    }
}
