using System.Collections.Generic;

namespace Hisab.UI.ViewModels
{
    public class AdminVm
    {
        public List<UserEventVm> Events { get; set; }

        public AdminVm()
        {
            Events = new List<UserEventVm>();
        }
    }

   
}
