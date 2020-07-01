using Hisab.BL;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Pages.Components.Feedback
{
    public class FeedbackViewComponent : ViewComponent
    {
        
        public FeedbackViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync(FeedBackItemVm feedBackVm)
        {
            return View("default", feedBackVm);
        }
    }
}
