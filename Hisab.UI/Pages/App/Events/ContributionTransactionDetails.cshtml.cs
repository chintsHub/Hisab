using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ContributionTransactionDetailsModel : PageModel
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventTransactionManager _transactionManager;

        [BindProperty]
        public ContributeVM ContributeVM { get; set; }

        public ContributionTransactionDetailsModel(IEventManager eventManager, UserManager<ApplicationUser> userManager, IEventTransactionManager transactionManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _transactionManager = transactionManager;

            
        }
        public async Task<IActionResult> OnGet(Guid Id, Guid transId)
        {
            ContributeVM = new ContributeVM();
            
            
            if (transId == Guid.Empty)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var eventAccount = await _transactionManager.GetEventAccount(Id);

                ContributeVM.NickName = user.NickName;
                ContributeVM.UserId = user.Id;
                ContributeVM.TransactionDate = DateTime.Now;
                ContributeVM.EventId = Id;
                ContributeVM.EventPoolId = eventAccount.AccountId;

            }
                
            return Page();
        }

        public async Task<IActionResult> OnGetToFriend(Guid Id, Guid transId)
        {
            ContributeVM = new ContributeVM();


            if (transId == Guid.Empty)
            {
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

            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var newTrans = new NewTransactionBO();

            if (ContributeVM.EventPoolId != null)
            {
                // Contribute to Pool
                newTrans.EventId = ContributeVM.EventId;
                newTrans.CreatedByUserId = ContributeVM.UserId;
                newTrans.PaidByUserId = ContributeVM.UserId;
                newTrans.TotalAmount = ContributeVM.Amount;
                newTrans.TransactionDate = ContributeVM.TransactionDate;
                newTrans.Description = ContributeVM.Description;
                newTrans.EventPoolAccountId = ContributeVM.EventPoolId.Value;

                var result = await _transactionManager.CreateContributeToPoolTransaction(newTrans);
            }

            if(ContributeVM.LendToFriendUserId != null)
            {
                // Contribute to Friend
                newTrans.EventId = ContributeVM.EventId;
                newTrans.CreatedByUserId = ContributeVM.UserId;
                newTrans.PaidByUserId = ContributeVM.UserId;
                newTrans.TotalAmount = ContributeVM.Amount;
                newTrans.TransactionDate = ContributeVM.TransactionDate;
                newTrans.Description = ContributeVM.Description;
                newTrans.LendToFriendUserId = ContributeVM.LendToFriendUserId.Value;

                var result = await _transactionManager.CreateContributeToFriend(newTrans);
            }

            return Page();
        }
    }
}