﻿using Sieve.Attributes;
using System;

namespace Hisab.UI.ViewModels
{
    public class UserEventVm 
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

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
    }

   
   
}
