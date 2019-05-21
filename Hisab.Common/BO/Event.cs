using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    public class EventBO
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public int UserId { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }
    }

    public class UserEventBO
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public int UserId { get; set; }

        public string Email { get; set; }


        public string NickName { get; set; }

        public string EventOwner { get; set; }

    }
}
