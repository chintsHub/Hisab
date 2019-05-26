using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common.BO;

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

    public class EventVm 
    {
        public int EventId { get; set; }

        
        public string EventName { get; set; }

        public List<EventFriendVm> Friends { get; set; }

        public EventFriendVm NewEventFriend { get; set; }


    }

    public class UserEventVm 
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string CreatedUserNickName { get; set; }



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
