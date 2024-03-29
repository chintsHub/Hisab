﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Hisab.UI
{
    [Authorize(Roles = "App User, Admin")]
    public class AddContributionTransactionModel : PageModel
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventTransactionManager _transactionManager;
        //private IToastNotification _toastNotification;

        [BindProperty]
        public ContributeVM ContributeVM { get; set; }

        public AddContributionTransactionModel(IEventManager eventManager, UserManager<ApplicationUser> userManager
                , IEventTransactionManager transactionManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _transactionManager = transactionManager;
            //_toastNotification = toastNotification;


        }
        public void OnGet(Guid Id)
        {
            

            
        }

        public async Task<IActionResult> OnGetToFriend(Guid Id)
        {
            ContributeVM = new ContributeVM();


                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var eve = await _eventManager.GetEventById(Id);

                ContributeVM.NickName = user.NickName;
                ContributeVM.UserId = user.Id;
                ContributeVM.TransactionDate = DateTime.Now;
                ContributeVM.EventId = Id;

                foreach(var friend in eve.Friends)
                {
                    if(friend.UserId != user.Id)
                    {
                        ContributeVM.Friends.Add(new EventFriendVm()
                        {
                            UserId = friend.UserId,
                            Name = friend.NickName

                        });
                    }
                }

            this.ViewData.Add("EventTitle", eve.EventName);

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            
            if(!ModelState.IsValid)
            {
                return Page();
            }
            
            var newTrans = new NewTransactionBO();

            

            if(ContributeVM.PaidToFriendUserId != null)
            {
                // Contribute to Friend
                newTrans.EventId = ContributeVM.EventId;
                newTrans.CreatedByUserId = ContributeVM.UserId;
                newTrans.PaidByUserId = ContributeVM.UserId;
                newTrans.TotalAmount = ContributeVM.Amount;
                newTrans.TransactionDate = ContributeVM.TransactionDate;
                newTrans.Description = ContributeVM.Description;
                newTrans.PaidToFriendUserId = ContributeVM.PaidToFriendUserId.Value;

                var result = await _transactionManager.CreateContributeToFriend(newTrans);
                if (result)
                {
                    //_toastNotification.AddSuccessToastMessage($"The Amount of {ContributeVM.Amount} is lent to your friend.");

                    return RedirectToPage("Dashboard", new { id = ContributeVM.EventId });
                }
            }

            return Page();
        }
    }
}