﻿using Hisab.UI.ViewModels;
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
            var currentPage = pages[pages.Count() - 1].ToLower();

            if (User.Identity.IsAuthenticated)
            {
                if(currentPage == "dashboard" || currentPage == "eventsettings" || currentPage == "eventfriends" || currentPage == "transactions")
                {
                    return View(EventMenu(path));
                }
                    return View(AppMenu(path));
            }

            return View(CreateDefaultNavigation(path));
        }

        private NavigationVM AppMenu(string path)
        {
            var nav = new NavigationVM();
            
            nav.Items.Add(new NavigationItemVM() { Page = "Events", Label = "Events", IsCurrentPage = path.Contains("Events") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Friends", Label = "Friends", IsCurrentPage = path.Contains("Friends") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Invites", Label = "Invites", IsCurrentPage = path.Contains("Invites") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Alerts", Label = "Alerts", IsCurrentPage = path.Contains("Alerts") ? true : false });

            return nav;
        }

        private NavigationVM EventMenu(string path)
        {
            var nav = new NavigationVM();

            
            nav.Items.Add(new NavigationItemVM() { Page = "Dashboard", Label = "Dashboard", IsCurrentPage = path.Contains("Events") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "EventSettings", Label = "Settings", IsCurrentPage = path.Contains("Settings") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "EventFriends", Label = "Friends", IsCurrentPage = path.Contains("Friends") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Transactions", Label = "Transactions", IsCurrentPage = path.Contains("Transactions") ? true : false });

            nav.Items.Add(new NavigationItemVM() { Page = "Events", Label = "My Events", IsCurrentPage = path.Contains("Events") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Friends", Label = "My Friends", IsCurrentPage = path.Contains("Friends") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Invites", Label = "My Invites", IsCurrentPage = path.Contains("Invites") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Alerts", Label = "Alerts", IsCurrentPage = path.Contains("Alerts") ? true : false });

            return nav;
        }

        private NavigationVM CreateDefaultNavigation(string path)
        {
            var nav = new NavigationVM();

            nav.Items.Add(new NavigationItemVM() { Page = "Index", Label="Features", IsCurrentPage=path.Contains("Features")?true:false});
            nav.Items.Add(new NavigationItemVM() { Page = "Pricing", Label = "Pricing", IsCurrentPage = path.Contains("Pricing") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Contact", Label = "Contact", IsCurrentPage = path.Contains("Contact") ? true : false });
            nav.Items.Add(new NavigationItemVM() { Page = "Register", Label="Register", IsCurrentPage = path.Contains("Register") ? true : false });

            return nav;
        }
    }
}
