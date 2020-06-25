using Hisab.Common.BO;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hisab.UI.ViewModels
{
   

    public class UserEventInviteVM
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public Guid EventId { get; set; }

        public InviteStatus InviteStatus { get; set; }

        public AvatarVm Avatar { get; set; }
    }


}
