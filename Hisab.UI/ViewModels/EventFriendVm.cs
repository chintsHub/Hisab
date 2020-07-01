using System;
using System.ComponentModel.DataAnnotations;
using Amazon.SimpleEmail.Model.Internal.MarshallTransformations;
using Hisab.Common.BO;

namespace Hisab.UI.ViewModels
{
    public class EventFriendVm
    {
        public Guid EventId { get; set; }

        public Guid UserId {get; set;}

        [Required(ErrorMessage = "Friend Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Friend Name is required")]
        public string Name { get; set; }

        public bool IsFriendActive { get; set; }

        public string Status { get; set; }

        public EventFriendStatus EventFriendStatus { get; set; }

        
        public AvatarVm Avatar { get; set; }
    }

    public class InviteFriendVm
    {
        [Required]
        public Guid EventId { get; set;  }
        
        [Required]
        public string FriendEmail { get; set; }
    }

    public class EventFriendExpensePaidVM : EventFriendVm
    {
        public decimal ExpensePaid { get; set; }

        public bool IsCurrentUser { get; set; }
        
    }

    public class EventFriendSharedVM : EventFriendVm
    {
        public bool IsShared { get; set; }

        public decimal SharePercent { get; set; }

    }
}
