using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Hisab.UI
{
    public class TransactionsModel : PageModel
    {
        IEventTransactionManager _transactionManager;
        IEventManager _eventManager;
        IToastNotification _toastNotification;

        public List<TransactionVM> Transactions { get; set; }

        [BindProperty]
        public DeleteTransactionVM DeleteTransactionVm { get; set; }

        public TransactionsModel(IEventTransactionManager transactionManager, IToastNotification toastNotification, IEventManager eventManager)
        {
            _transactionManager = transactionManager;
            _toastNotification = toastNotification;
            _eventManager = eventManager;

            Transactions = new List<TransactionVM>();
        }
        public async Task<IActionResult> OnGet(Guid Id)
        {
            var eve = await _eventManager.GetEventById(Id);
            this.ViewData.Add("EventTitle", eve.EventName);

            if (await _eventManager.CheckEventAccess(eve, User.Identity.Name))
            {
                var trans = await _transactionManager.GetTransactions(Id);

                foreach (var tran in trans)
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


                    if (tran.TransactionType == Common.BO.TransactionType.Expense || tran.TransactionType == Common.BO.TransactionType.ExpenseFromPool)
                    {
                        foreach (var friend in tran.SharedWith)
                        {
                            tranVM.SharedWith += friend.NickName + ",";
                        }

                    }

                    if (tran.TransactionType == Common.BO.TransactionType.ExpenseFromPool)
                    {
                        tran.PaidByName += "Using Money Pool";
                    }

                    if (tran.TransactionType == Common.BO.TransactionType.ContributionToPool)
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

            throw new UnauthorizedAccessException();
        }

        public async Task<IActionResult> OnPostDeletTransaction()
        {

            var retVal = await _transactionManager.DeleteTransaction(DeleteTransactionVm.TransactionId);

            if(retVal)
            {
                _toastNotification.AddSuccessToastMessage("Transaction deleted successfully");
                return new JsonResult(new { success = true, responseText = "Transaction Deleted" });
            }

            return new JsonResult(new { success = false, responseText = "Transaction not Deleted" });
        }

            
        
    }
}