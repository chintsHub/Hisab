using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.Extensions;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI
{
    public class CustomerStoriesModel : PageModel
    {
        private IFeedbackManager _feedbackManager { get; set; }

        public List<FeedBackItemVm> Feeds { get; set; }
        public CustomerStoriesModel(IFeedbackManager feedbackManager )
        {
            _feedbackManager = feedbackManager;
            Feeds = new List<FeedBackItemVm>();
        }
        public async Task<IActionResult> OnGet()
        {
            var feedBos = await _feedbackManager.GetTestimonyFeedBack();

            foreach(var f in feedBos)
            {
                Feeds.Add(new FeedBackItemVm
                {
                    FeedbackDate = f.FeedbackDate,
                    Id = f.Id,
                    FeedbackTypeName = f.FeedbackType.GetDescription(),
                    NickName = f.NickName,
                    Message = f.Message
                });
            }

            return Page();
        }
    }
}