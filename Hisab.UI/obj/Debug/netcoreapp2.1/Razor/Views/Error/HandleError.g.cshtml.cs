#pragma checksum "C:\Chintan\Code\Hisab\Hisab.UI\Views\Error\HandleError.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8c34c7abf9d0502badca87c646de3fa2b6ceb4c7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Error_HandleError), @"mvc.1.0.view", @"/Views/Error/HandleError.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Error/HandleError.cshtml", typeof(AspNetCore.Views_Error_HandleError))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8c34c7abf9d0502badca87c646de3fa2b6ceb4c7", @"/Views/Error/HandleError.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e44ae01019361e377f0244715cd5b6d7561d01a3", @"/Views/_ViewImports.cshtml")]
    public class Views_Error_HandleError : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Chintan\Code\Hisab\Hisab.UI\Views\Error\HandleError.cshtml"
  
    ViewData["Title"] = "HandleError";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
            BeginContext(96, 31, true);
            WriteLiteral("\r\n<h2>Error</h2>\r\n\r\n<div>\r\n    ");
            EndContext();
            BeginContext(128, 24, false);
#line 10 "C:\Chintan\Code\Hisab\Hisab.UI\Views\Error\HandleError.cshtml"
Write(ViewData["ErrorMessage"]);

#line default
#line hidden
            EndContext();
            BeginContext(152, 23, true);
            WriteLiteral("\r\n</div>\r\n\r\n<div>\r\n    ");
            EndContext();
            BeginContext(176, 28, false);
#line 14 "C:\Chintan\Code\Hisab\Hisab.UI\Views\Error\HandleError.cshtml"
Write(ViewData["RouteOfException"]);

#line default
#line hidden
            EndContext();
            BeginContext(204, 18, true);
            WriteLiteral("\r\n</div>\r\n\r\n\r\n\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
