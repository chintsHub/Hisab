using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Hisab.UI.ViewModels
{
    public class RegisterUserVm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class FeedbackVm
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public FeedbackType Feedback { get; set; }

       
        public IEnumerable<SelectListItem> FeedbackList { get; }

        public FeedbackVm()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Value = FeedbackType.Issue.ToString(), Text = FeedbackType.Issue.GetDescription() });
            list.Add(new SelectListItem() { Value = FeedbackType.Suggestion.ToString(), Text = FeedbackType.Suggestion.GetDescription() });
            list.Add(new SelectListItem() { Value = FeedbackType.Testimony.ToString(), Text = FeedbackType.Testimony.GetDescription() });

            FeedbackList = list.AsEnumerable();

        }
    }

   
}
