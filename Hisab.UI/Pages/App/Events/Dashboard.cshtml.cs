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

namespace Hisab.UI
{
    [Authorize(Roles = "App User, Admin")]
    public class DashboardModel : PageModel
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IEventTransactionManager _eventTransactionManager;

        [BindProperty(SupportsGet =true)]
        public EventVm Event { get; set; }

        
        public bool IsLoggedInUserTheEventAdmin { get; set; }

        public DashboardModel(IEventManager eventManager, UserManager<ApplicationUser> userManager, IEventTransactionManager eventTransactionManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _eventTransactionManager = eventTransactionManager;

        }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            
            var eve = await _eventManager.GetEventById(Id);

            if (await _eventManager.CheckEventAccess(eve, User.Identity.Name))
            {
                Event = new EventVm()
                {
                    EventId = eve.Id,
                    EventName = eve.EventName,
                    CurrencySymbol = Currency.GetCurrencySymbolFromCode(eve.CurrencyCode,true)
                };

                this.ViewData.Add("EventTitle", Event.EventName);

                foreach (var f in eve.Friends)
                {
                    Event.Friends.Add(new EventFriendVm()
                    {
                        EventId = f.EventId,
                        UserId = f.UserId,
                        Email = f.Email,
                        Name = f.NickName,
                        IsFriendActive = f.IsFriendActive,
                        Status = f.EventFriendStatus.GetDescription(),
                        Avatar = HisabImageManager.GetAvatar(f.Avatar)
                    });
                }

                var eventAdmin = eve.Friends.Where(x => x.EventFriendStatus == Common.BO.EventFriendStatus.EventAdmin).First().Email;

                if (eventAdmin.ToLower() == User.Identity.Name.ToLower())
                    IsLoggedInUserTheEventAdmin = true;

                return Page();
            }

            throw new UnauthorizedAccessException();
        }

        public async Task<IActionResult> OnGetMyExpense(Guid Id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var expBalance = await _eventTransactionManager.GetExpenseAccountBalance(Id, user.Id);

            return new JsonResult(new { balance = expBalance});
        }

        public async Task<IActionResult> OnGetAllExpense(Guid Id)
        {
            
            var totalExpense = await _eventTransactionManager.GetTotalExpense(Id);

            return new JsonResult(new { totalExpense = totalExpense });
        }

        public async Task<IActionResult> OnGetAmountIOweToFriends(Guid Id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var amountIOwe = await _eventTransactionManager.GetAmountIOweToFriends(Id,user.Id);

            return new JsonResult(new { amountIOwe = amountIOwe });
        }

        public async Task<IActionResult> OnGetAmountFriendsOweToMe(Guid Id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var amountFriendsOwe = await _eventTransactionManager.GetAmountFriendsOweToMe(Id, user.Id);

            return new JsonResult(new { amountFriendsOwe = amountFriendsOwe });
        }

        public async Task<IActionResult> OnGetMyContributions(Guid Id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var myContributions = await _eventTransactionManager.GetMyContributions(Id, user.Id);

            return new JsonResult(new { myContributions = myContributions });
        }

       
    }
}