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
        [Description("Admin")]
        EventAdmin = 1,



        [Description("Friend")]
        EventFriend = 2,

    }

    public enum InviteStatus
    {
        RequestPending = 1,
        RequestAccepted = 2,
        RequestRejected = 3
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

        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public EventFriendStatus EventFriendStatus { get; set; }

        public bool IsFriendActive { get; set; }



        public AvatarEnum Avatar { get; set; }

        public EventFriendBO()
        {

        }
    }

    public class UserEventBO
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }

        public DateTime CreateDate { get; set; }

        public EventStatus EventStatus { get; set; }

        public int EventPic { get; set; }

        public string OwnerName { get; set; }
        public Guid OwnerUserId { get; set; }



    }

    public class NewInviteBO
    {
        public string UserEmail { get; set; }

        public Guid EventId { get; set; }

        EventFriendStatus Status { get; set; }
    }

    public class UserEventInviteBO
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string EventName { get; set; }

        public string EventOwnerName { get; set; }

        public Guid EventId { get; set; }

        public int EventPic { get; set; }

        public AvatarEnum AvatarId { get; set; }

        public InviteStatus InviteStatus { get; set; }
    }

    public class EventBO
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }

        public int EventPicId { get; set; }

        public EventStatus EventStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public List<EventFriendBO> Friends { get; set; }

        //public EventDashboardStatBo DashboardStats { get; set; }

        //http://taylorhutchison.com/2016/03/23/dapper-orm-complex-queries.html
    }

    

    public class EventSettingsBO
    {
        public Guid EventId { get; set; }

        public string EventName { get; set; }

        public int SelectedEventImage { get; set; }

        public List<EventFriendBO> Friends { get; set; }

        public EventSettingsBO()
        {
            Friends = new List<EventFriendBO>();
        }
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

    public class ManagerResponse
    {
        public string Messge { get; set; }

        public bool Success { get; set; }

        public Exception Exception { get; set; }
    }

}
