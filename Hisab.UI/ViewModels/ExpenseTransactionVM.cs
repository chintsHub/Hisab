using Hisab.Common.BO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.ViewModels
{
    public class TransactionVM
    {
        public Guid EventId { get; set; }

        

        public Guid TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public string TransactionDescription { get; set; }

        public string PaidByName { get; set; }
        public Guid PaidById { get; set; }
        public string PaidByEmail { get; set; }

        public TransactionType TransactionType { get; set; }

        public string SharedWith { get; set; }

        public Decimal Amount { get; set; }

        public TransactionVM()
        {
            
        }

    }

    public class DeleteTransactionVM
    {
        public Guid TransactionId { get; set; }

        public Guid EventId { get; set; }
    }

    public class ExpenseTransactionVM
    {
        public Guid TransactionId { get; set; }

        public Guid EventId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ExpenseDescription { get; set; }

        
        public Guid ExpensePaidById { get; set; }
        public decimal ExpensePaid { get; set; }
        public List<PaidByVM> PaidByList { get; set; }

        public TransactionType TransactionType { get; set; }

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

    public class ContributeVM
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }
        public string NickName { get; set; }

        public Decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public TransactionType TransactionType { get; set; }

        public Guid? LendToFriendUserId { get; set; }

        public Guid? EventPoolId { get; set; }

        public List<EventFriendVm> Friends { get; set; }

        public ContributeVM()
        {
            Friends = new List<EventFriendVm>();
        }
    }

    public class SettlementAccountVM
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid FriendId { get; set; }

        public string FriendName { get; set; }

        public AvatarVm FriendAvatar { get; set; }

        public decimal AmountPayable { get; set; }

        public decimal AmountReceivable { get; set; }

        public decimal NetAmount { get; set; }
    }

    public class SettlementTransaction
    {
        public Guid EventId { get; set; }

        public DateTime TransactionDate { get; set; }

        public Guid PaidByUserId { get; set; }

        public Guid PaidToUserId { get; set; }

        public decimal Amount { get; set; }
    }


}
