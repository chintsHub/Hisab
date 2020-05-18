using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sieve.Attributes;

namespace Hisab.UI.ViewModels
{
    
    public class EventCardVm
    {
        public Guid EventId { get; set; }

        public string EventName { get; set; }

        public string CreatedUserNickName { get; set; }

        public string EventImagePath { get; set; }

        public string EventMessage { get; set; }

    }
    public class EventVm 
    {
        public Guid EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        public string SelectedEventStatus { get; set; }

        public IEnumerable<SelectListItem> EventStatusList { get; }

        public List<EventFriendVm> Friends { get; set; }

        public EventFriendVm NewEventFriend { get; set; }


        public NewSplitByFriendVm NewSplitByFriendVm { get; set; }
        public NewEventPoolEntryVm NewEventPoolEntry { get; set; }

        public decimal TotalEventExpense { get; set; }

        public decimal TotalEventPoolBalance { get; set; }

        public decimal MyEventExpense { get; set; }

        public decimal MyContributions { get; set; }

        public decimal MyNetAmount { get; set; }

        public List<TransactionVm> Transactions { get; set; }

        public SettlementHeaderVm SettlementHeader { get; set; }


        public EventVm()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem(){Value = EventStatus.Active.ToString(),Text = EventStatus.Active.GetDescription()});
            list.Add(new SelectListItem(){Value = EventStatus.Archived.ToString(), Text = EventStatus.Archived.GetDescription() });

            EventStatusList = list.AsEnumerable();

            Transactions = new List<TransactionVm>();
            Friends = new List<EventFriendVm>();
        }


    }

    public class EventSettingsVM
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public Guid EventId { get; set; }

        public List<HisabImage> EventImages { get; set; }

       
        public int SelectedEventImage { get; set; }

        public List<EventFriendVm> Friends { get; set; }

        public EventSettingsVM()
        {
            EventImages = new List<HisabImage>();

            EventImages = HisabImageManager.GetEventImages();

            Friends = new List<EventFriendVm>();
        }
    }

    public class SettlementHeaderVm
    {
        public Dictionary<string,int> Payers { get; set; }

        public Dictionary<string,int> Receivers { get; set; }

        public List<SettlementVm> Settlements { get; set; }

        public int Columns { get; set; }
        public int Rows { get; set; }

        public SettlementHeaderVm()
        {
            Payers = new Dictionary<string, int>();
            Receivers = new Dictionary<string, int>();
            Settlements = new List<SettlementVm>();
        }
    }

    public class SettlementVm
    {
        public int EventId { get; set; }

        public string PayerFriend { get; set; }
        public int PayerRowId { get; set; }

        public string ReceiverFriend { get; set; }
        public int ReceiverColumnId { get; set; }

        public decimal Amount { get; set; }
    }


    public class NewSplitByFriendVm
    {
        public decimal MaxAllowedPoolAmount { get; set; }

        public decimal PaidByPoolAmount { get; set; }

        [Required]
        public string Description { get; set; }

        public List<NewSplitByFriendDetailsVm> FriendDetails { get; set; }

        public int EventId { get; set; }

        public NewSplitByFriendVm()
        {
            FriendDetails = new List<NewSplitByFriendDetailsVm>();
        }


    }

    public class NewSplitByFriendDetailsVm
    {
        public int EventFriendId { get; set; }

        
        public string Name { get; set; }

        public decimal AmountPaid { get; set; }

        public bool IncludeInSplit { get; set; }
    }

    public class TransactionVm
    {
        public int TransactionId { get; set; }

        
        public string Description { get; set; }

        public decimal TotalAmount { get; set; }

        public string SplitType { get; set; }

       
        public Guid CreatedByUserId { get; set; }
        public string NickName { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }

    public class DeleteTransactionRequest
    {
        public List<int> TransactionId { get; set; }
    }


    public class NewEventPoolEntryVm
    {

        public int EventId { get; set; }

        public decimal ContributionAmount { get; set; }

        public IEnumerable<SelectListItem> FriendList { get; set; }
        public int SelectedFriendId { get; set; }

        public NewEventPoolEntryVm()
        {
            FriendList = new List<SelectListItem>();

            
            
        }
    }

    public class FeedBackItemVm
    {
        public Guid Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string NickName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string UserName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Message { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string FeedbackTypeName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime FeedbackDate { get; set; }

        public bool ShowAsTestimony { get; set; }
    }

    public class UpdateTestimonyVm
    {
        public Guid Id { get; set; }

        public bool ShowAsTestimony { get; set; }
    }

    public class NewEventVm
    {
        
        [Required]
        public string EventName { get; set; }

        public string Url { get; set; }
    }

}
