using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class IndexModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/App/Events");
            }

            return Page();
        }
    }
}