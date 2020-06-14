using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hisab.UI
{
    public class EventSettlementModel : PageModel
    {
        private IEventTransactionManager _eventTransactionManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventManager _eventManager;

        public List<SettlementAccountVM> SettlementAccounts { get; set; }

        public EventSettlementModel(UserManager<ApplicationUser> userManager, IEventManager eventManager, IEventTransactionManager eventTransactionManager)
        {
            _eventTransactionManager = eventTransactionManager;
            _userManager = userManager;
            _eventManager = eventManager;

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
                    settlementVm.FriendAvatar = HisabImageManager.GetAvatar(settlement.FriendAvatar);
                    settlementVm.FriendName = settlement.FriendName;
                    settlementVm.AmountPayable = settlement.AccountPayable.CalculateBalance();
                    settlementVm.AmountReceivable = settlement.AccountReceivable.CalculateBalance();
                    settlementVm.NetAmount = Math.Abs(settlementVm.AmountReceivable) - Math.Abs(settlementVm.AmountPayable);

                    SettlementAccounts.Add(settlementVm);
                }

                return Page();
            }

            throw new UnauthorizedAccessException();

        }
    }
}