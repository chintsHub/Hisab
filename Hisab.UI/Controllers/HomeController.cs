using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hisab.UI.Controllers
{
    public class HomeController : Controller
    {
        private IDbConnectionProvider _connectionProvider;
        private SignInManager<ApplicationUser> _signInManager;

        public HomeController(IDbConnectionProvider connectionProvider, SignInManager<ApplicationUser> signInManager)
        {
            _connectionProvider = connectionProvider;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, false);

                if (result.Succeeded)
                {
                    //redirect
                }
            }

            return null;
        }
    }
}
