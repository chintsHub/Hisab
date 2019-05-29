using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hisab.UI.ViewModels
{
   

    public class LoginVM
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterUserVm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string NickName { get; set; }
    }

    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }

    public class ResetPasswordVm
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class HomePageVM
    {
        public LoginVM LoginVm { get; set; }

        public RegisterUserVm RegisterUserVm { get; set; }

        public ForgotPasswordVM ForgotPasswordVm { get; set; }

        public ResetPasswordVm ResetPasswordVm { get; set; }
    }

    public class EmailServiceCredentials
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }
    }

    public class NewEvent
    {
        public string EventName { get; set; }

       
    }

    public class AppHomeVm
    {
        public NewEvent NewEvent { get; set; }

        public List<UserEventVm> userEvents { get; set; }
    }

    public class EventInviteVm
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventOwner { get; set; }

        [Required]
        public int EventFriendId { get; set; }
    }

    public class InviteVm
    {
        public List<EventInviteVm> Invites { get; set; }

       
    }

    public class EventVm 
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        public string SelectedStatus { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; }

        public List<EventFriendVm> Friends { get; set; }

        public EventFriendVm NewEventFriend { get; set; }

        public EventVm()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem(){Value = EventStatus.Active.ToString(),Text = EventStatus.Active.GetDescription()});
            list.Add(new SelectListItem(){Value = EventStatus.Inactive.ToString(), Text = EventStatus.Inactive.GetDescription() });

            StatusList = list.AsEnumerable();
        }


    }

    public class UserEventVm 
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string CreatedUserNickName { get; set; }



    }

    public class AdminVm
    {
        public List<UserEventVm> Events { get; set; }

        public AdminVm()
        {
            Events = new List<UserEventVm>();
        }
    }

    public class UserSettingsVm
    {
        public ChangePasswordVm ChangePasswordVm { get; set; }

        public UpdateNickNameVm UpdateNickNameVm { get; set; }
    }

    public class UpdateNickNameVm
    {
        public string NickName { get; set; }
    }

    public class ChangePasswordVm
    {
        
        public string NewPassword { get; set; }
    }

    public class EventFriendVm
    {
        public int EventId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public int KidsCount { get; set; }

        public int AdultCount { get; set; }

        public EventFriendStatus Status { get; set; }
    }

   
}
