using Sieve.Attributes;
using System;

namespace Hisab.UI.ViewModels
{
    public class UserEventVm 
    {
        public Guid EventId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string EventName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string CreatedUserNickName { get; set; }



    }

    public class ApplicationUserVm
    {
        public Guid Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string UserName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public bool EmailConfirmed { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string NickName { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public bool IsUserActive { get; set; }

        public AvatarVm Avatar { get; set; }
    }

   
    public class InviteApplicationUserVM : ApplicationUserVm
    {
        public bool Checked { get; set; }

        public Guid EventId { get; set; }
    }
}
