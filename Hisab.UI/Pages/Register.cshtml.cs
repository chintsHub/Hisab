﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class RegisterModel : PageModel
    {
        public RegisterUserVm RegisterUserVm { get; set; }

        public void OnGet()
        {

        }
    }
}