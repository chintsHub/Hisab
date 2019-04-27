using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hisab.AWS;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private IEmailService _emailService;
        private ILogger _logger;

        public HomeController(IDbConnectionProvider connectionProvider, 
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            IEmailService emailService
            )
        {
            _connectionProvider = connectionProvider;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }
        public IActionResult Index(string returnUrl = null)
        {
            //setting up ReturnUrl
            ViewData["ReturnUrl"] = returnUrl;

            

            if (!User.Identity.IsAuthenticated)
                return View();
            else
            {
                return RedirectToAction("Index", "AppHome");
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
                        return RedirectToAction("Index", "AppHome");
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
                    EmailConfirmed = false,
                    UserName = registerUserVm.Email,
                    NickName = registerUserVm.NickName

                };
                

                var result = await _userManager.CreateAsync(user, registerUserVm.Password);
                if (result.Succeeded)
                {
                    // Add to role
                    var roleResult = await _userManager.AddToRoleAsync(user, "App User");

                    // send email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var emailResponse = await _emailService.SendRegistrationEmail(user.Email, callbackUrl);

                   
                    if (roleResult.Succeeded && emailResponse == HttpStatusCode.OK)
                    {
                        ViewBag.RegisterSuccessMessage = "You are successfully registered." +
                                                         "We have sent a verification email.";

                        ModelState.Clear();
                        return View("Index", new HomePageVM());
                        
                    }
                    else
                    {
                        throw new ApplicationException($"Error in Registering user:{user.Email}");
                    }

                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Index", new HomePageVM(){RegisterUserVm = registerUserVm});
           
        }

        public async Task<IActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId <= 0 || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
                return View("ConfirmEmail");


            return RedirectToAction("HandleError","Error",new {StatusCode = 500});

        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    
                   
                    var resetUrl = Url.ResetPasswordCallbackLink(user.Id, token,Request.Scheme);


                    //send email
                    var emailResponse = await _emailService.SendForgotPasswordLink(user.Email, resetUrl);

                    if (emailResponse == HttpStatusCode.OK)
                    {
                        ViewBag.ForgotPasswordSuccessMessage = "We have sent a reset password link to your email.";

                        ModelState.Clear();
                        return View("Index", new HomePageVM());
                    }
                    else
                    {
                        throw new ApplicationException("We couldn't send you forgot password link");
                    }

                    
                }
                else
                {
                    //send email - You are not registered with hisab.io
                }

                
            }

            return View("Index", new HomePageVM() { ForgotPasswordVm = forgotPasswordVm });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token = null)
        {
            if(String.IsNullOrEmpty(token))
            {
                throw new ApplicationException("You must provide a token");
            }

            var resetPasswordVM = new ResetPasswordVm() { Token = token };
            return View("ResetPassword", resetPasswordVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm model)
        {
            ViewBag.ResetSuccessMessage = "";

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new ApplicationException("We could not reset your password.");
                
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.ResetSuccessMessage = "Your password is successfully updated.";
            }
            AddErrors(result);
            return View();
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
