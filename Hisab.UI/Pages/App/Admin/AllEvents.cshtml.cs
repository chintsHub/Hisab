using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI.Pages.App.Admin
{
    public class AllEventsModel : PageModel
    {
        private IEventManager _eventManager;
        private IFilterProcessor _filterProcessor;

        [BindProperty(SupportsGet = true)]
        public string ApplicationBasePath { get; set; }

        public AllEventsModel(IEventManager eventManager, IFilterProcessor filterProcessor, IHostingEnvironment environment)
        {
            _eventManager = eventManager;
            _filterProcessor = filterProcessor;
                       
        }
        public void OnGet()
        {

        }

        public async Task<JsonResult> OnGetLoadData(FilterOptions model)
        {
            var request = Request;
            
            var events = await _eventManager.GetAllEvents();
            var eventsVM = new List<UserEventVm>();
            foreach (var e in events)
            {
                eventsVM.Add(new UserEventVm()
                {
                    CreatedUserNickName = e.OwnerName,
                    EventId = e.Id,
                    EventName = e.EventName
                });
            }

            var returnValue = _filterProcessor.Process(eventsVM.AsQueryable(), model);




            return new JsonResult(returnValue.Value);
        }
    }
}