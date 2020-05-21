using Hisab.Common.BO;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hisab.UI.ViewModels
{
    public class EventInviteVm
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventOwner { get; set; }

        [Required]
        public int EventFriendId { get; set; }
    }

    public class UserEventInviteVM
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public Guid EventId { get; set; }

        public InviteStatus InviteStatus { get; set; }

        public AvatarVm Avatar { get; set; }
    }


}
