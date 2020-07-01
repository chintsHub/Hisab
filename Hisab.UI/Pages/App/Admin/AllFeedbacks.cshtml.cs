using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI.Pages.App.Admin
{
    [Authorize(Roles = "Admin")]
    public class AllFeedbacksModel : PageModel
    {
        private IFeedbackManager _feedbackManager;
        private IFilterProcessor _filterProcessor;

        [BindProperty]
        public UpdateTestimonyVm UpdateTestimonyVm { get; set; }

        public AllFeedbacksModel(IFeedbackManager feedbackManager, IFilterProcessor filterProcessor)
        {
            _feedbackManager = feedbackManager;
            _filterProcessor = filterProcessor;
        }
        public void OnGet()
        {

        }

        public async Task<JsonResult> OnGetLoadData(FilterOptions model)
        {

            var feedbacks = await _feedbackManager.GetAllFeedbacks();

            var Feeds = new List<FeedBackItemVm>();

            foreach (var f in feedbacks)
            {
                Feeds.Add(new FeedBackItemVm
                {
                    FeedbackDate = f.FeedbackDate.Date,
                    Id = f.Id,
                    FeedbackTypeName = f.FeedbackType.GetDescription(),
                    NickName = f.NickName,
                    Message = f.Message,
                    UserName = f.UserName,
                    ShowAsTestimony = f.ShowAsTestimony
                });
            }

            var returnValue = _filterProcessor.Process(Feeds.AsQueryable(), model);


            return new JsonResult(returnValue.Value);
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _feedbackManager.UpdateTestimony(UpdateTestimonyVm.Id, UpdateTestimonyVm.ShowAsTestimony);

            if(result)
                return new OkResult();


            return BadRequest();
        }
    }
}