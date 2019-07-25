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


        public EventVm()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem(){Value = EventStatus.Active.ToString(),Text = EventStatus.Active.GetDescription()});
            list.Add(new SelectListItem(){Value = EventStatus.Inactive.ToString(), Text = EventStatus.Inactive.GetDescription() });

            EventStatusList = list.AsEnumerable();

            
        }


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

    

   
}
