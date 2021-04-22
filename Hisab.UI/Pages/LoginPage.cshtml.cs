using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common.Log;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using Serilog.Events;

namespace Hisab.UI
{
    public class LoginPageModel : PageModel
    {
        private SignInManager<ApplicationUser> _signInManager;
        

        [BindProperty]
        public LoginVM loginVm { get; set; }

        public LoginPageModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            
        }

        public void OnGet(string returnUrl = null)
        {
            loginVm = new LoginVM();

            //setting up ReturnUrl
            ViewData["ReturnUrl"] = returnUrl;
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage("/LoginPage");
        }

        public async Task<IActionResult> OnPostAsync(string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, false);

               

                if (result.Succeeded)
                {
                    Log.Write(LogEventLevel.Information, "{@LogDetail}", LogHelper.CreateLogDetail(LogType.Usage, "User Logged in", username: loginVm.Email));


                    // Returnurl - or can be passed in as Login method parameter
                    if (!String.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToPage("/App/Events");
                        
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    
                                       
                }
            }

            return Page();
        }



    }
}