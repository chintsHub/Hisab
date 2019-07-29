﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hisab.UI.ViewModels
{
    public class EventVm 
    {
        public int EventId { get; set; }

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
            list.Add(new SelectListItem(){Value = EventStatus.Inactive.ToString(), Text = EventStatus.Inactive.GetDescription() });

            EventStatusList = list.AsEnumerable();

            Transactions = new List<TransactionVm>();
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

       
        public int CreatedByUserId { get; set; }
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
   
}
