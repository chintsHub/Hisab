using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class TransactionsModel : PageModel
    {
        IEventTransactionManager _transactionManager;

        public List<TransactionVM> Transactions { get; set; }

        public TransactionsModel(IEventTransactionManager transactionManager)
        {
            _transactionManager = transactionManager;

            Transactions = new List<TransactionVM>();
        }
        public async Task<IActionResult> OnGet(Guid Id)
        {
            var trans = await _transactionManager.GetTransactions(Id);

            foreach(var tran in trans)
            {
                var tranVM = new TransactionVM()
                {
                    EventId = tran.EventId,
                    TransactionId = tran.TransactionId,
                    TransactionDate = tran.TransactionDate,
                    TransactionDescription = tran.TransactionDescription,
                    Amount = tran.TotalAmount,
                    PaidById = tran.PaidById,
                    PaidByName = tran.PaidByName,
                    TransactionType = tran.TransactionType,
                    PaidByEmail = tran.PaidByEmail
                };


                if(tran.TransactionType == Common.BO.TransactionType.Expense || tran.TransactionType == Common.BO.TransactionType.ExpenseFromPool)
                {
                    foreach (var friend in tran.SharedWith)
                    {
                        tranVM.SharedWith += friend.NickName + ",";
                    }
                    
                }

                if(tran.TransactionType == Common.BO.TransactionType.ExpenseFromPool)
                {
                    tran.PaidByName += "Using Money Pool";
                }
               
                if(tran.TransactionType == Common.BO.TransactionType.ContributionToPool)
                {
                    tranVM.SharedWith = "Money Pool";
                }

                if (tran.TransactionType == Common.BO.TransactionType.LendToFriend)
                {
                    tranVM.SharedWith = tran.LendToFriendName;
                }

                Transactions.Add(tranVM);

            }

            return Page();
        }
    }
}