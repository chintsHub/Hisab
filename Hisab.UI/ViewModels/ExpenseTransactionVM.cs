using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.ViewModels
{
    public class ExpenseTransactionVM
    {
        public Guid TransactionId { get; set; }

        public Guid EventId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ExpenseDescription { get; set; }

        
        public Guid ExpensePaidById { get; set; }
        public decimal ExpensePaid { get; set; }
        public List<PaidByVM> PaidByList { get; set; }


        public decimal ExpensePaidFromPool { get; set; }

        public decimal MaxAllowedPoolAmount { get; set; }

        public List<EventFriendSharedVM> ExpenseSharedWith { get; set; }

        public ExpenseTransactionVM()
        {
            ExpenseSharedWith = new List<EventFriendSharedVM>();
            PaidByList = new List<PaidByVM>();

            ExpenseDate = DateTime.Today;
        }
    }

    public class PaidByVM
    {
        public Guid Id { get; set; }

        public String Name { get; set; }
    }
}
