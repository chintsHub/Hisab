using System.ComponentModel.DataAnnotations;
using Hisab.Common.BO;

namespace Hisab.UI.ViewModels
{
    public class EventFriendVm
    {
        public int EventId { get; set; }

        public int EventFriendId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public int KidsCount { get; set; }

        public int AdultCount { get; set; }

        public EventFriendStatus Status { get; set; }
    }

   
}
