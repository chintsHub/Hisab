using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.Dapper.Identity;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public FeedbackVm FeedbackVm { get; set; }

        public string SucessMessage { get; set; }

        private UserManager<ApplicationUser> _userManager { get; set; }
        private IFeedbackManager _feedbackManager { get; set; }

        public ContactModel(UserManager<ApplicationUser> userManager, IFeedbackManager feedbackManager)
        {
            _userManager = userManager;
            _feedbackManager = feedbackManager;
        }
        public async Task<IActionResult> OnGet()
        {

            FeedbackVm = new FeedbackVm();

            return Page();

        }

        public async Task<IActionResult> OnPost()
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);



                var feedback = new NewFeedbackBO
                {
                    Message = FeedbackVm.Message,
                    UserId = user.Id,
                    FeedbackType = FeedbackVm.Feedback
                };

                var result = await _feedbackManager.CreateNewFeedback(feedback);

                if(result)
                {
                    ModelState.Clear();
                    SucessMessage = "Thank you for providing feedback.";
                }
            }
            

            return Page();

        }
    }
}