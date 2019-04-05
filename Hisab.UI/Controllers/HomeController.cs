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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using WebApp.Services;

namespace Hisab.UI.Controllers
{
    public class HomeController : Controller
    {
        private IDbConnectionProvider _connectionProvider;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private IEmailSender _emailSender;
        private ILogger _logger;

        public HomeController(IDbConnectionProvider connectionProvider, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger
            //IEmailSender emailSender
            )
        {
            _connectionProvider = connectionProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            //_emailSender = emailSender;
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
                    _logger.LogInformation("User logged in successfully");

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
                    return View("Index", new HomePageVM(){LoginVm = loginVm});
                }
            }

            // If we got this far, something failed, redisplay form
            
            return View("Index",new HomePageVM());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVm registerUserVm, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = registerUserVm.Email,
                    EmailConfirmed = true,
                    UserName = registerUserVm.Email,
                    NickName = registerUserVm.NickName

                };
                var result = await _userManager.CreateAsync(user, registerUserVm.Password);
                if (result.Succeeded)
                {
                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(registerUserVm.Email, callbackUrl);
                    var roleResult = await _userManager.AddToRoleAsync(user, "App User");
                    if (roleResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    
                    //_logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal("/AppHome");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            //return View(registerUserVm);
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            return null;
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
