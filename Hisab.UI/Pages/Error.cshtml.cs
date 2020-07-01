using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace Hisab.UI.Pages
{
    public class ErrorModel : PageModel
    {
        private IHostingEnvironment _hostingEnvironment { get; set; }
        public string RequestId { get; set; }

        public string Referrer { get; set; }

        public string EnvironmentName { get; set; }

        public string ExceptionMessage { get; set; }
        public bool ShowExceptionMessage => !string.IsNullOrEmpty(ExceptionMessage);

        public string InnerExceptionMessage { get; set; }
        public bool ShowInnerExceptionMessage => !string.IsNullOrEmpty(InnerExceptionMessage);

        public string StackTrace { get; set; }
        public bool ShowStackTrace => !string.IsNullOrEmpty(StackTrace);

        public ErrorModel(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Referrer = HttpContext.Request.Headers["referer"];

            EnvironmentName = _hostingEnvironment.EnvironmentName;

            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if(exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ExceptionMessage = exceptionHandlerPathFeature.Error.Message;

                if (_hostingEnvironment.EnvironmentName.ToLower() != "production" && exceptionHandlerPathFeature.Error.InnerException != null)
                {
                    InnerExceptionMessage = exceptionHandlerPathFeature.Error.InnerException.ToString();

                    StackTrace = exceptionHandlerPathFeature.Error.StackTrace;
                }
                Log.Error(exceptionHandlerPathFeature.Error, "RequestId=" + RequestId);
            }
            
               

           
        }

        public void OnPost()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Referrer = HttpContext.Request.Headers["referer"];

            EnvironmentName = _hostingEnvironment.EnvironmentName;

            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ExceptionMessage = exceptionHandlerPathFeature.Error.Message;

                if (_hostingEnvironment.EnvironmentName.ToLower() != "production" && exceptionHandlerPathFeature.Error.InnerException != null)
                {
                    InnerExceptionMessage = exceptionHandlerPathFeature.Error.InnerException.ToString();

                    StackTrace = exceptionHandlerPathFeature.Error.StackTrace;
                }

                Log.Error(exceptionHandlerPathFeature.Error, "RequestId=" + RequestId);
            }

            

        }
    }
}