using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Common.BO
{
    public class NewEventBO
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public EventFriendBO EventOwner { get; set; }

        

        public NewEventBO()
        {
            
            CreateDateTime = DateTime.Now;
            LastModifiedDateTime = DateTime.Now;
        }
    }

    public enum EventFriendStatus
    {
        EventOwner = 1,
        PendingRegistration,
        PendingAcceptance,
        EventFriend
    }

    public class EventFriendBO
    {

        public int EventFriendId { get; set; }

        public int EventId { get; set; }

        public int? UserId { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public int AdultCount { get; set; }

        public int KidsCount { get; set; }

        public EventFriendStatus Status { get; set; }

        public EventFriendBO()
        {
            AdultCount = 1;
            KidsCount = 0;
        }
    }

    public class UserEventBO
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string NickName { get; set; }



    }

    public class EventBO
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public List<EventFriendBO> Friends { get; set; }

        //http://taylorhutchison.com/2016/03/23/dapper-orm-complex-queries.html
    }
}
