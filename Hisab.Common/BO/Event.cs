using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Hisab.Common.BO
{
    public class NewEventBO
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

        public NewEventFriendBO EventOwner { get; set; }

        public EventStatus Status { get; set; }

        public HisabImage EventPic { get; set; }

        
        
    }

    public class NewEventFriendBO
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public EventFriendStatus Status { get; set; }
    }

    public enum EventFriendStatus
    {
        Owner = 1,
        RequestToJoin = 2, 
        RequestAccepted = 3,
        Inactive = 4,
        RequestDeclined = 5
    }

    public enum EventStatus
    {
        [Description("Active")]
        Active = 1,

        [Description("Archived")]
        Archived
    }

    public enum FeedbackType
    {
        [Description("Suggestion")]
        Suggestion = 1,

        [Description("Issue")]
        Issue = 2,

        [Description("Testimony")]
        Testimony = 3
    }

    public class NewFeedbackBO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Message { get; set; }

        public FeedbackType FeedbackType { get; set; }

        public DateTime FeedbackDate { get; set; }

        public bool? ShowAsTestimony { get; set; }
    }

    public class FeedBackBO
    {
        public Guid Id { get; set; }

        public string NickName { get; set; }

        public string Message { get; set; }

        public FeedbackType FeedbackType { get; set; }

        public DateTime FeedbackDate { get; set; }

        public string UserName { get; set; }

        public bool ShowAsTestimony { get; set; }
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
        public Guid EventId { get; set; }

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

    public enum AvatarEnum
    {
        [Description("Default Avatar")]
        Default = 1,

        [Description("Boy casual")]
        Boy1 = 2,

        [Description("Girl casual")]
        Girl1 = 3,

        [Description("BatGirl")]
        GirlSuperhero1 = 4,

        [Description("Batman")]
        BoySuperhero1 = 5,

    }

    public class HisabImage
    {
        public int Id { get; set; }

        public string ImageName { get; set; }

        public string ImagePath { get; set; }
    }

    public class UserSettingsBO
    {
        public string NickName { get; set; }

        public AvatarEnum Avatar { get; set; }

        public bool IsUserActive { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
