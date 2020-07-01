using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI.Pages.App.Admin
{
    [Authorize(Roles = "Admin")]
    public class SystemSettingsModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}