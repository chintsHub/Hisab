using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NToastNotify;

namespace Hisab.UI
{
    public class EventSettlementModel : PageModel
    {
        private IEventTransactionManager _eventTransactionManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventManager _eventManager;
        private IToastNotification _toastNotification;

        public List<SettlementAccountVM> SettlementAccounts { get; set; }

        [BindProperty]
        public SettlementTransaction SettlementTransaction { get; set; }

        public EventSettlementModel(UserManager<ApplicationUser> userManager, IEventManager eventManager, IEventTransactionManager eventTransactionManager, IToastNotification toastNotification)
        {
            _eventTransactionManager = eventTransactionManager;
            _userManager = userManager;
            _eventManager = eventManager;
            _toastNotification = toastNotification;
            SettlementAccounts = new List<SettlementAccountVM>();
        }
        public async Task<IActionResult> OnGet(Guid Id)
        {
            var eve = await _eventManager.GetEventById(Id);
            this.ViewData.Add("EventTitle", eve.EventName);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (await _eventManager.CheckEventAccess(eve, User.Identity.Name))
            {
                var settlements = await _eventTransactionManager.GetSettlementAccounts(eve, user.Id);

                foreach(var settlement in settlements)
                {
                    var settlementVm = new SettlementAccountVM();

                    settlementVm.EventId = settlement.EventId;
                    settlementVm.UserId = user.Id;
                    settlementVm.FriendId = settlement.FriendId;
                    settlementVm.FriendAvatar = HisabImageManager.GetAvatar(settlement.FriendAvatar);
                    settlementVm.FriendName = settlement.FriendName;
                    settlementVm.AmountPayable = settlement.AccountPayable.CalculateBalance();
                    settlementVm.AmountReceivable = Math.Abs(settlement.AccountReceivable.CalculateBalance());
                    settlementVm.NetAmount = Math.Abs(settlementVm.AmountReceivable) - Math.Abs(settlementVm.AmountPayable);

                    SettlementAccounts.Add(settlementVm);
                }

                return Page();
            }

            throw new UnauthorizedAccessException();

        }

        public async Task<IActionResult> OnPostSettlementTransaction()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var settlementTrans = new NewTransactionBO()
            {
                EventId = SettlementTransaction.EventId,
                CreatedByUserId = user.Id,
                Description = $"Settlement - from {user.NickName}",
                PaidToFriendUserId = SettlementTransaction.PaidToUserId,
                TotalAmount = Math.Abs(SettlementTransaction.Amount),
                TransactionDate = DateTime.Now,
                PaidByUserId = user.Id,
                TransactionType = TransactionType.Settlement
                

            };

            var retVal = await _eventTransactionManager.CreateSettlementTransaction(settlementTrans);

            if (retVal)
            {
                _toastNotification.AddSuccessToastMessage("Settlement Transaction created successfully");
                return new JsonResult(new { success = true, 
                                    responseText = $"Congratulations !! You have successfully settled your dues. We have created a settlement transaction which can be found on Transaction page.", 
                                    userId = user.Id,
                                    friendId = SettlementTransaction.PaidToUserId });
            }

            return new JsonResult(new { success = false, responseText = "Error", userId = user.Id, friendId = SettlementTransaction.PaidToUserId });
        }
    }
}