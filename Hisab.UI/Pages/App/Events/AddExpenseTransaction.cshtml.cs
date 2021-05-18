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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Hisab.UI
{
    [Authorize(Roles = "App User, Admin")]
    public class AddExpenseTransactionModel : PageModel
    {

        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventTransactionManager _transactionManager;
        //private IToastNotification _toastNotification;

        [BindProperty(SupportsGet = true)]
        public ExpenseTransactionVM ExpenseVM { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public AddExpenseTransactionModel(IEventManager eventManager, UserManager<ApplicationUser> userManager, IEventTransactionManager transactionManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _transactionManager = transactionManager;
            //_toastNotification = toastNotification;


        }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            await LoadViewModel(Id);

            return Page();
        }

        private async Task LoadViewModel(Guid Id)
        {
            ExpenseVM = new ExpenseTransactionVM();
            var eve = await _eventManager.GetEventById(Id);
            


            ExpenseVM.EventId = Id;


            foreach (var f in eve.Friends.OrderBy(x => x.NickName).Where(f => f.IsFriendActive))
            {
                if (f.Email.ToLower() == User.Identity.Name.ToLower())
                {
                    ExpenseVM.ExpensePaidById = f.UserId;

                    ExpenseVM.PaidByList.Add(new PaidByVM() { Id = f.UserId, Name = f.NickName });
                    
                   

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

            this.ViewData.Add("EventTitle", eve.EventName);
        }

        public async Task<IActionResult> OnPost()
        {
            bool hasError = false;
           

            if (!ModelState.IsValid)
                hasError = true;
            else
            {
                var newTrans = new NewTransactionBO();
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                newTrans.CreatedByUserId = user.Id;
                newTrans.Description = ExpenseVM.ExpenseDescription;
                newTrans.EventId = ExpenseVM.EventId;
                newTrans.TransactionDate = ExpenseVM.ExpenseDate;
                newTrans.TotalAmount = ExpenseVM.ExpensePaid;
                newTrans.PaidByUserId = ExpenseVM.ExpensePaidById;


                foreach (var split in ExpenseVM.ExpenseSharedWith)
                {
                    if (split.IsShared)
                    {
                        var splitBO = new TransactionSplitBO();
                        splitBO.EventId = ExpenseVM.EventId;
                        splitBO.UserId = split.UserId;
                        newTrans.TransactionSplits.Add(splitBO);
                    }

                }
                var result = await _transactionManager.CreateExpenseTransaction(newTrans);

                if (result.Success)
                {
                    SuccessMessage = result.Messge;
                    //_toastNotification.AddSuccessToastMessage(result.Messge);

                    return RedirectToPage("Dashboard", new { id = ExpenseVM.EventId });
                }
                else
                {
                    ErrorMessage = result.Messge;
                    hasError = true;
                }
            }

            if(hasError)
            {
                //reload model
                await LoadViewModel(ExpenseVM.EventId);
            }

            return Page();
        }
    }
}