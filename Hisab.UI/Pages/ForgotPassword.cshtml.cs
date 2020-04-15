using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hisab.AWS;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public ForgotPasswordVM ForgotPasswordVM { get; set; }

        [BindProperty]
        public string SucessMessage { get; set; }

        private UserManager<ApplicationUser> _userManager;
        private IEmailService _emailService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(ForgotPasswordVM.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);


                    var resetUrl = Url.ResetPasswordCallbackLink(user.Id, token, Request.Scheme);


                    //send email
                    var emailResponse = await _emailService.SendForgotPasswordLink(user.Email, user.NickName, resetUrl);

                    if (emailResponse == HttpStatusCode.OK)
                    {
                        SucessMessage = "We have sent a reset password link to your email.";

                        ModelState.Clear();
                        
                    }
                    else
                    {
                        ModelState.AddModelError("","We couldn't send you forgot password link");
                    }


                }
                else
                {
                    ModelState.AddModelError("", "This email address is not registered with Hisab.");
                }


            }

            return Page();
        }
    }
}