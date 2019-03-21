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
        public IActionResult Index(string returnUrl = null)
        {
            //setting up ReturnUrl
            ViewData["ReturnUrl"] = returnUrl;

            if (!User.Identity.IsAuthenticated)
                return View();
            else
            {
                return RedirectToLocal("/AppHome");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVm)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe, false);

                if (result.Succeeded)
                {
                    //redirect

                    // Returnurl - or can be passed in as Login method parameter
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    else
                    {
                        return RedirectToLocal("/AppHome");
                    }
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("Index", loginVm);
                }
            }

            // If we got this far, something failed, redisplay form
            return View("Index",loginVm);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
               // return RedirectToAction(returnUrl);
               return RedirectToAction("Index", "AppHome");
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
