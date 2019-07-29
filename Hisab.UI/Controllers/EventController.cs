using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.Extensions;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NToastNotify;

namespace Hisab.UI.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private IEventManager _eventManager;
        private UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification;
        

        

        public EventController(IToastNotification toastNotification, IEventManager eventManager, UserManager<ApplicationUser> userManager)
        {
            _eventManager = eventManager;
            _userManager = userManager;
            _toastNotification = toastNotification;
            
        }

        private async Task<bool> CanAccessEvent(EventBO eventbo)
        {
            if (User.IsInRole("Admin"))
                return true;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = _eventManager.CheckEventAccess(eventbo, user.Id);

            return result;
        }

        [HttpGet]
        [Route("/Event/Dashboard/{eventId}")]
        public async Task<IActionResult> Dashboard(int eventId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);
            

            if (await CanAccessEvent(eventBo))
            {
                eventBo.DashboardStats = await _eventManager.GetDashboardStats(eventId, eventBo.Friends.First(x => x.AppUserId == user.Id).EventFriendId);
                
                return View("Index", BuildFriendList(eventBo));
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");

        }

        [HttpGet]
        [Route("Event/Friends/{eventId}")]
        public async Task<IActionResult> Friends(int eventId)
        {
            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);

            if (await CanAccessEvent(eventBo))
            {
                return View("Friends", BuildFriendList(eventBo));
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");

        }

        private EventVm BuildFriendList(EventBO eventBo)
        {

            var friendList = new List<EventFriendVm>();
            var splitByFriend = new NewSplitByFriendVm();
            var eventPoolEntry = new NewEventPoolEntryVm();
            var friendListItems = new List<SelectListItem>();

            splitByFriend.EventId = eventBo.EventId;

            foreach (var friend in eventBo.Friends)
            {
                friendList.Add(new EventFriendVm()
                {
                    Name = friend.NickName,
                    Email = friend.Email,
                    AdultCount = friend.AdultCount,
                    EventId = eventBo.EventId,
                    KidsCount = friend.KidsCount,
                    Status = friend.Status,
                    EventFriendId = friend.EventFriendId
                });

                splitByFriend.FriendDetails.Add(new NewSplitByFriendDetailsVm()
                    {
                        EventFriendId = friend.EventFriendId,
                        IncludeInSplit = true,
                        Name = friend.NickName

                    }


                );

                friendListItems.Add(new SelectListItem(){Value = friend.EventFriendId.ToString(),Text = friend.NickName});
            }

            eventPoolEntry.FriendList = friendListItems.AsEnumerable();
            eventPoolEntry.EventId = eventBo.EventId;

            var eve = new EventVm()
            {
                EventId = eventBo.EventId,
                EventName = eventBo.EventName,
                Friends = friendList,
                NewSplitByFriendVm = splitByFriend,
                NewEventPoolEntry = eventPoolEntry

            };


            if (eventBo.DashboardStats != null)
            {
                eve.MyContributions = eventBo.DashboardStats.MyContributions;
                eve.TotalEventExpense = eventBo.DashboardStats.TotalEventExpense;
                eve.TotalEventPoolBalance = eventBo.DashboardStats.TotalEventPoolBalance;
                eve.MyNetAmount = eventBo.DashboardStats.MyNetAmount;
                eve.MyEventExpense = eventBo.DashboardStats.MyEventExpense;
                eve.NewSplitByFriendVm.MaxAllowedPoolAmount = eve.TotalEventPoolBalance;
            }

            
            

            return eve;
        }

        [HttpPost]
        public async Task<IActionResult> DisableFriend(EventFriendVm eventFriendVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var canAccess = await _eventManager.CanAccessEvent(eventFriendVm.EventId, user.Id);

            if (canAccess)
            {
                var result = await _eventManager.DisableFriend(eventFriendVm.EventFriendId);

                if (result)
                    _toastNotification.AddSuccessToastMessage("Friend made InActive");

                return RedirectToAction("Friends", new { eventFriendVm.EventId });
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFriend(EventFriendVm eventFriendVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var canAccess = await _eventManager.CanAccessEvent(eventFriendVm.EventId, user.Id);

            if (canAccess)
            {
                if (ModelState.IsValid)
                {
                    var eventFriendBo = new EventFriendBO()
                    {
                        EventId = eventFriendVm.EventId,
                        EventFriendId = eventFriendVm.EventFriendId,
                        AdultCount = eventFriendVm.AdultCount,
                        Email = eventFriendVm.Email,
                        KidsCount = eventFriendVm.KidsCount,
                        Status = eventFriendVm.Status,
                        NickName = eventFriendVm.Name
                    };

                    var result = await _eventManager.UpdateFriend(eventFriendBo);

                    if (result)
                        _toastNotification.AddSuccessToastMessage("Friend updated sucessfully");
                }

                return RedirectToAction("Friends", new { eventFriendVm.EventId });
            }


            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        [HttpGet]
        [Route("Event/Transactions/{eventId}")]
        public async Task<IActionResult> Transactions(int eventId)
        {
            var eventBo = await _eventManager.GetEventById(eventId);

            if (await CanAccessEvent(eventBo))
            {

                var results = await _eventManager.GetAllTransactions(eventId);
                var transList = new List<TransactionVm>();
                foreach (var transaction in results)
                {
                    transList.Add(new TransactionVm()
                    {
                        CreatedDateTime = transaction.CreatedDateTime,
                        NickName = transaction.NickName,
                        TotalAmount = transaction.TotalAmount,
                        SplitType = transaction.SplitType.GetDescription(),
                        CreatedByUserId = transaction.CreatedByUserId,
                        Description = transaction.Description,
                        TransactionId = transaction.Id
                    });
                }
                return View("Transactions", new EventVm() { EventId = eventBo.EventId, EventName = eventBo.EventName,Transactions = transList});
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        [HttpGet]
        [Route("Event/Settlement/{eventId}")]
        public async Task<IActionResult> Settlement(int eventId)
        {
            var eventBo = await _eventManager.GetEventById(eventId);

            if (await CanAccessEvent(eventBo))
            {
                var results = await _eventManager.GetSettlementData(eventId);
                var settlementHeaderVm = new SettlementHeaderVm();
                int columnId = 1; //1st column is taken by list of payers
                int rowId = 0;

                foreach (var settlement in results)
                {
                    if (!settlementHeaderVm.Payers.ContainsKey(settlement.PayerFriend))
                    {
                        rowId++;
                        settlementHeaderVm.Payers.Add(settlement.PayerFriend,rowId);
                    }
                        

                    if (!settlementHeaderVm.Receivers.ContainsKey(settlement.ReceiverFriend))
                    {
                        columnId++;
                        settlementHeaderVm.Receivers.Add(settlement.ReceiverFriend, columnId);
                        
                    }

                    var s = new SettlementVm()
                    {
                        Amount = settlement.Amount,
                        EventId = settlement.EventId,
                        PayerFriend = settlement.PayerFriend,
                        ReceiverFriend = settlement.ReceiverFriend,
                        ReceiverColumnId = settlementHeaderVm.Receivers.GetValueOrDefault(settlement.ReceiverFriend),
                        PayerRowId = settlementHeaderVm.Payers.GetValueOrDefault(settlement.PayerFriend)
                    };
                    settlementHeaderVm.Settlements.Add(s);

                }

               

                settlementHeaderVm.Rows = rowId;
                settlementHeaderVm.Columns = columnId;

                return View("Settlement", new EventVm() { EventId = eventBo.EventId, EventName = eventBo.EventName, SettlementHeader = settlementHeaderVm });
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        [HttpGet]
        [Route("Event/Settings/{eventId}")]
        public async Task<IActionResult> Settings(int eventId)
        {
            //Load event
            var eventBo = await _eventManager.GetEventById(eventId);

            if (await CanAccessEvent(eventBo))
            {
                var eventVm = new EventVm() { EventId = eventBo.EventId, EventName = eventBo.EventName, Friends = null };
                eventVm.EventStatusList.FirstOrDefault(x => x.Text == eventBo.Status.GetDescription()).Selected = true;

                return View("Settings", eventVm);
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        public async Task<IActionResult> UpdateEvent(EventVm eventVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var canAccess = await _eventManager.CanAccessEvent(eventVm.EventId, user.Id);

            if (canAccess)
            {
                if (ModelState.IsValid)
                {
                    EventStatus status;
                    Enum.TryParse(eventVm.SelectedEventStatus, out status);
                    var result = await _eventManager.UpdateEvent(eventVm.EventName, eventVm.EventId, status);

                    if (result)
                    {
                        _toastNotification.AddSuccessToastMessage("Event name updated sucessfully.");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Couldn't update event name");
                        return View("Settings", eventVm);
                    }


                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Invalid data passed");
                    return View("Settings", eventVm);
                }

                return RedirectToAction("Settings", new { eventVm.EventId });
            }

            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }

        [HttpPost]
        public async Task<IActionResult> AddSplitByFriend(NewSplitByFriendVm NewSplitByFriendVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var trans = (SplitPerFriendTransactionBo)TransactionFactory.CreateNewSplitTransaction(SplitType.SplitPerFriend);

            trans.CreatedByUserId = user.Id;
            trans.CreatedDateTime = DateTime.UtcNow;

            trans.EventId = NewSplitByFriendVm.EventId;
            trans.Description = NewSplitByFriendVm.Description;
            foreach (var friend in NewSplitByFriendVm.FriendDetails)
            {
                trans.Friends.Add(new SplitPerFriendBo(){ AmountPaid = friend.AmountPaid, EventFriendId = friend.EventFriendId, IncludeInSplit = friend.IncludeInSplit});
            }

            trans.PaidByPoolAmount = NewSplitByFriendVm.PaidByPoolAmount;
            trans.SplitType = SplitType.SplitPerFriend;

            var transId = _eventManager.ProcessTransaction(trans);

            if(transId > 0)
                _toastNotification.AddSuccessToastMessage("Expense added");
            else
                _toastNotification.AddErrorToastMessage("Could not process transaction");


            return RedirectToAction("Dashboard", "Event", new { NewSplitByFriendVm.EventId});
            
        }

        [HttpPost]
        public async Task<IActionResult> AddContribution(NewEventPoolEntryVm newEventPoolEntryVm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var trans = TransactionFactory.CreatePoolEntryTransaction();

            trans.CreatedByUserId = user.Id;
            trans.CreatedDateTime = DateTime.UtcNow;

            trans.EventId = newEventPoolEntryVm.EventId;
            trans.Description = "Cash contributed by " + newEventPoolEntryVm.SelectedFriendId;
            trans.SplitType = SplitType.NotApplicable;
            trans.TotalAmount = newEventPoolEntryVm.ContributionAmount;
            ((EventPoolTransactionBo) trans).EventFriendId = newEventPoolEntryVm.SelectedFriendId;

            var transId = _eventManager.ProcessTransaction(trans);

            if (transId > 0)
                _toastNotification.AddSuccessToastMessage("Expense added");
            else
                _toastNotification.AddErrorToastMessage("Could not process transaction");


            return RedirectToAction("Dashboard", "Event", new { newEventPoolEntryVm.EventId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(EventVm eventv)
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(EventFriendVm newEventFriend)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var canAccess = await _eventManager.CanAccessEvent(newEventFriend.EventId, user.Id);

            if (canAccess)
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        var newFriend = await _eventManager.CreateEventFriend(new EventFriendBO()
                        {
                            Email = newEventFriend.Email,
                            AdultCount = newEventFriend.AdultCount,
                            EventId = newEventFriend.EventId,
                            KidsCount = newEventFriend.KidsCount,
                            NickName = newEventFriend.Name

                        });

                        if (newFriend)
                            _toastNotification.AddSuccessToastMessage($"Sucessfully added Friend {newEventFriend.Name} to this event.");


                    }
                    catch (Exception ex)
                    {
                        if (ex is HisabException)
                        {
                            _toastNotification.AddErrorToastMessage("Error:" + ex.Message);

                        }
                        else
                        {
                            throw;
                        }
                    }

                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Invalid data passed");
                }




                return RedirectToAction("Friends", new { newEventFriend.EventId });
            }


            _toastNotification.AddErrorToastMessage("You dont have permission to access the event");
            return RedirectToAction("Index", "AppHome");
        }
    }

    
}