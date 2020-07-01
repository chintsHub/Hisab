using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Pages.Components.Navigation
{
    public class NavigationViewComponent : ViewComponent
    {
        private IHttpContextAccessor _httpContextAccessor;

        

        public NavigationViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var path = _httpContextAccessor.HttpContext.Request.Path.ToUriComponent();
                        
            var pages = path.Split("/");
            //var currentPage = pages[pages.Count() - 1].ToLower();

            if (User.Identity.IsAuthenticated)
            {
                if(path.Contains("Admin"))
                {
                    return View(AdminMenu(path));
                }
                
                if(pages.Count() > 3)
                {
                    return View(EventMenu(path, Guid.Parse(pages[3])));
                }
                
                return View(AppMenu(path));
            }

            return View(CreateDefaultNavigation(path));
        }

        private NavigationVM AppMenu(string path)
        {
            var nav = new NavigationVM();
            
            nav.Items.Add(new NavigationItemVM() { Page = "Events", Label = "Events", IsCurrentPage = path.Contains("Events") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Invites", Label = "Invites", IsCurrentPage = path.Contains("Invites") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Contact", Label = "Feedback", IsCurrentPage = path.Contains("Feedback") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "UserSettings", Label = "Settings", IsCurrentPage = path.Contains("Settings") ? true : false });

            return nav;
        }

        private NavigationVM AdminMenu(string path)
        {
            var nav = new NavigationVM();

            nav.Items.Add(new NavigationItemVM() { Page = "AllUsers", Label = "All Users", IsCurrentPage = path.Contains("AllUsers") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "AllEvents", Label = "All Events", IsCurrentPage = path.Contains("AllEvents") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "AllFeedbacks", Label = "All Feedbacks", IsCurrentPage = path.Contains("AllFeedbacks") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "SystemSettings", Label = "System", IsCurrentPage = path.Contains("System") ? true : false });

            return nav;
        }

        private NavigationVM EventMenu(string path, Guid EventId)
        {
            var nav = new NavigationVM();

            
            nav.Items.Add(new NavigationItemVM() { Page = "Dashboard", EventId= EventId, Label = "Dashboard", IsCurrentPage = path.Contains("dashboard") ? true : false, IsEventMenu=true });
            
            nav.Items.Add(new NavigationItemVM() { Page = "Transactions", EventId = EventId, Label = "Transactions", IsCurrentPage = path.Contains("transactions") ? true : false, IsEventMenu = true });
            nav.Items.Add(new NavigationItemVM() { Page = "EventSettings", EventId = EventId, Label = "Settings", IsCurrentPage = path.Contains("settings") ? true : false, IsEventMenu = true });
            nav.Items.Add(new NavigationItemVM() { Page = "EventSettlement", EventId = EventId, Label = "Settlement", IsCurrentPage = path.Contains("EventSettlement") ? true : false, IsEventMenu = true });

            nav.Items.Add(new NavigationItemVM() { Page = "/app/events", Label = "My Events", IsCurrentPage = path.Contains("Events") ? true : false });
            
            nav.Items.Add(new NavigationItemVM() { Page = "/app/Invites", Label = "My Invites", IsCurrentPage = path.Contains("Invites") ? true : false });
            

            return nav;
        }

        private NavigationVM CreateDefaultNavigation(string path)
        {
            var nav = new NavigationVM();

            nav.Items.Add(new NavigationItemVM() { Page = "Index", Label="Features", IsCurrentPage=path.Contains("Features")?true:false});
            nav.Items.Add(new NavigationItemVM() { Page = "Pricing", Label = "Pricing", IsCurrentPage = path.Contains("Pricing") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "CustomerStories", Label = "User Stories", IsCurrentPage = path.Contains("Stories") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Register", Label="Register", IsCurrentPage = path.Contains("Register") ? true : false });

            return nav;
        }
    }
}
