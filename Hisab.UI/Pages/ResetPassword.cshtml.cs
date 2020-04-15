using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty(SupportsGet =true)]
        public ResetPasswordVm ResetPasswordVm { get; set; }
        
        [BindProperty]
        public string SuccessMessage { get; set; }

        private UserManager<ApplicationUser> _userManager { get; set; }

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public void OnGet()
        {
            if (String.IsNullOrEmpty(ResetPasswordVm.Token))
            {
                ModelState.AddModelError("", "You must provide a token");
            }
            
            
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(ResetPasswordVm.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid user");

                }
                else
                {
                    var result = await _userManager.ResetPasswordAsync(user, ResetPasswordVm.Token, ResetPasswordVm.Password);
                    if (result.Succeeded)
                    {
                        SuccessMessage = "Your password is successfully updated.";
                    }
                    else
                    {
                        AddErrors(result);
                    }
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