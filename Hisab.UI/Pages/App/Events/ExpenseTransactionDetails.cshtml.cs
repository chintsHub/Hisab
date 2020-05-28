using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ExpenseTransactionDetailsModel : PageModel
    {

        private IEventManager _eventManager;

        [BindProperty]
        public ExpenseTransactionVM ExpenseVM { get; set; }

        public ExpenseTransactionDetailsModel(IEventManager eventManager)
        {
            _eventManager = eventManager;

            ExpenseVM = new ExpenseTransactionVM();
        }

        public async Task<IActionResult> OnGet(Guid Id, Guid transId)
        {
            if(transId == Guid.Empty)
            {
                var eve = await _eventManager.GetEventById(Id);

                ExpenseVM.EventId = Id;
                

                foreach (var f in eve.Friends.OrderBy(x => x.NickName))
                {
                    if (f.Email.ToLower() == User.Identity.Name.ToLower())
                    {
                        ExpenseVM.ExpensePaidById = f.UserId;

                        ExpenseVM.PaidByList.Add(new PaidByVM() { Id = f.UserId, Name = f.NickName });
                        ExpenseVM.PaidByList.Add(new PaidByVM() { Id = System.Guid.NewGuid(), Name = "Event Account" });
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
            }
            else
            {
                // transaction under edit mode
            }


            return Page();
        }

        public async Task<IActionResult> OnPost()
        {


            return Page();
        }
    }
}