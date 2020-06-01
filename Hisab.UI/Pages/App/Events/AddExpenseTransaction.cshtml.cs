using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Hisab.UI
{
    public class AddExpenseTransactionModel : PageModel
    {

        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventTransactionManager _transactionManager;
        private IToastNotification _toastNotification;

        [BindProperty]
        public ExpenseTransactionVM ExpenseVM { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public AddExpenseTransactionModel(IEventManager eventManager, UserManager<ApplicationUser> userManager, IEventTransactionManager transactionManager, IToastNotification toastNotification)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _transactionManager = transactionManager;
            _toastNotification = toastNotification;


        }

        public async Task<IActionResult> OnGet(Guid Id)
        {
                ExpenseVM = new ExpenseTransactionVM();
                var eve = await _eventManager.GetEventById(Id);
                var eventAccount = await _transactionManager.GetEventAccount(Id);


                ExpenseVM.EventId = Id;
                

                foreach (var f in eve.Friends.OrderBy(x => x.NickName))
                {
                    if (f.Email.ToLower() == User.Identity.Name.ToLower())
                    {
                        ExpenseVM.ExpensePaidById = f.UserId;

                        ExpenseVM.PaidByList.Add(new PaidByVM() { Id = f.UserId, Name = f.NickName });
                        var eventBalance = eventAccount.CalculateBalance();
                        if (eventBalance > 0)
                        {
                            ExpenseVM.PaidByList.Add(new PaidByVM() { Id = eventAccount.AccountId, Name = $"Event Account ({eventBalance})" });
                        }
                        
                    }
                    ExpenseVM.ExpenseSharedWith.Add(new EventFriendSharedVM()
                    {
                            EventId = f.EventId,
                            UserId = f.UserId,
                            Email = f.Email,
                            Name = f.NickName,
                            Status = f.EventFriendStatus.GetDescription(),
                            EventFriendStatus = f.EventFriendStatus,
                            IsFriendActive = f.IsFriendActive,
                            Avatar = HisabImageManager.GetAvatar(f.Avatar),
                            IsShared = true

                    });

                   

                }
            

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var newTrans = new NewTransactionBO();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            newTrans.CreatedByUserId = user.Id;
            newTrans.Description = ExpenseVM.ExpenseDescription;
            newTrans.EventId = ExpenseVM.EventId;
            newTrans.TransactionDate = ExpenseVM.ExpenseDate.Date;
            newTrans.TotalAmount = ExpenseVM.ExpensePaid;
            newTrans.PaidByUserId = ExpenseVM.ExpensePaidById;
            

            foreach(var split in ExpenseVM.ExpenseSharedWith)
            {
                if(split.IsShared)
                {
                    var splitBO = new TransactionSplitBO();
                    splitBO.EventId = ExpenseVM.EventId;
                    splitBO.UserId = split.UserId;
                    newTrans.TransactionSplits.Add(splitBO);
                }
                
            }
            var result = await _transactionManager.CreateExpenseTransaction(newTrans);

            if(result.Success)
            {
                SuccessMessage = result.Messge;
                _toastNotification.AddSuccessToastMessage(result.Messge);

                return RedirectToPage("Dashboard", new { id = ExpenseVM.EventId });
            }
            else
            {
                ErrorMessage = result.Messge;
            }

            return await OnGet(ExpenseVM.EventId);
        }
    }
}