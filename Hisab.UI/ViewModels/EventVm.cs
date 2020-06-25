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

        

        public IEnumerable<SelectListItem> EventStatusList { get; }

        public List<EventFriendVm> Friends { get; set; }

        

        


        public EventVm()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem(){Value = EventStatus.Active.ToString(),Text = EventStatus.Active.GetDescription()});
            list.Add(new SelectListItem(){Value = EventStatus.Archived.ToString(), Text = EventStatus.Archived.GetDescription() });

            EventStatusList = list.AsEnumerable();

           
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
