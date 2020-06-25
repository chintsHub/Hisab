using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Page(pageName: "ConfirmEmail",pageHandler:null, values: new { userId, code }, protocol: scheme);
                    
                
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, Guid userId, string token, string scheme)
        {
            return urlHelper.Page(pageName: "ResetPassword", pageHandler: null,values: new { userId, token }, protocol: scheme);
        }
    }
}
