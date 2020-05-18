using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.ViewModels
{
    public class NavigationVM
    {
        public List<NavigationItemVM> Items { get; set; }

        public NavigationVM()
        {
            Items = new List<NavigationItemVM>();
        }
    }

    public class NavigationItemVM
    {
        public string Label { get; set; }

        public string Page { get; set; }

        public bool IsCurrentPage { get; set; }

        public int BadgeCount { get; set; }

        public bool IsEventMenu { get; set; }
    }
}
