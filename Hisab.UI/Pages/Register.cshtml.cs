using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hisab.AWS;
using Hisab.BL;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;
        private IUserSettingManager _userSettingManager;

        public RegisterUserVm RegisterUserVm { get; set; }
        public string SucessMessage { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, IEmailService emailService, IUserSettingManager userSettingManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _userSettingManager = userSettingManager;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync(RegisterUserVm registerUserVm, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = registerUserVm.Email,
                    EmailConfirmed = false,
                    UserName = registerUserVm.Email,
                    NickName = registerUserVm.Name,
                    IsUserActive = true

                };


                var result = await _userManager.CreateAsync(user, registerUserVm.Password);
                if (result.Succeeded)
                {
                    // Add to role
                    var roleResult = await _userManager.AddToRoleAsync(user, "App User");

                    // send email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var emailResponse = await _emailService.SendRegistrationEmail(user.Email, user.NickName, callbackUrl);

                    // create accounts
                    var newUser = await _userManager.FindByNameAsync(registerUserVm.Email);
                    var accountResult = await _userSettingManager.CreateUserAccounts(newUser.Id);

                    if (roleResult.Succeeded && emailResponse == HttpStatusCode.OK && accountResult)
                    {
                        SucessMessage = "You are successfully registered. " +
                                                         "We have sent a verification email.";

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,$"Error in Registering user:{user.Email}");
                    }

                }
                else
                {
                    AddErrors(result);
                }

                
            }

            return Page();
          
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);

            }
        }
    }
}