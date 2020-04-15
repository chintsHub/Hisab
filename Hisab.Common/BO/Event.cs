using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public EventStatus Status { get; }

        

        public NewEventBO()
        {
            
            CreateDateTime = DateTime.Now;
            LastModifiedDateTime = DateTime.Now;
            Status = EventStatus.Active;
        }
    }

    public enum EventFriendStatus
    {
        EventOwner = 1,
        PendingRegistration,
        PendingAcceptance,
        EventFriend, //user is not registered with Hisab, its place holder friend
        EventJoined,
        Inactive
    }

    public enum EventStatus
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive
    }

    public class EventFriendBO
    {

        public int EventFriendId { get; set; }

        public int EventId { get; set; }

        public Guid? AppUserId { get; set; }

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

        public EventStatus Status { get; set; }

    }

    public class EventBO
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public EventStatus Status { get; set; }

        public List<EventFriendBO> Friends { get; set; }

        public EventDashboardStatBo DashboardStats { get; set; }

        //http://taylorhutchison.com/2016/03/23/dapper-orm-complex-queries.html
    }

    public class EventDashboardStatBo
    {
        
        public decimal TotalEventExpense { get; set; }

        public decimal TotalEventPoolBalance { get; set; }

        public decimal MyEventExpense { get; set; }

        public decimal MyContributions { get; set; }

        public decimal MyNetAmount { get; set; }

        
    }

    public class EventInviteBO
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventOwner { get; set; }

        public int EventFriendId { get; set; }
    }
}
