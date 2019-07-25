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

   
}
