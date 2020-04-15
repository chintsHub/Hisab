using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Dapper.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ConfirmEmailModel : PageModel
    {

        [BindProperty(SupportsGet =true)]
        public Guid userId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string code { get; set; }

        private UserManager<ApplicationUser> _userManager { get; set; }

        public ConfirmEmailModel(UserManager<ApplicationUser> UserManager)
        {
            _userManager = UserManager;
        }
        
        public async Task<IActionResult> OnGet()
        {
            
            if (userId == null || code == null)
            {
                ModelState.AddModelError("", "Invalid user or code");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid user");
                return Page();
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
                return Page();
            else
                AddErrors(result);


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