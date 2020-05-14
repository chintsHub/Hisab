using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;

namespace Hisab.UI
{
    public class EventSettingsModel : PageModel
    {
        [BindProperty(SupportsGet =true)]
        public EventSettingsVM SettingsVM { get; set; }

        public string EventSettingMessage { get; set; }

        private IEventManager _eventManager;

        public EventSettingsModel(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            SettingsVM = new EventSettingsVM();

            var eve = await _eventManager.GetEventById(Id);
            SettingsVM.EventName = eve.EventName;
            SettingsVM.SelectedEventImage = eve.EventPicId;
            SettingsVM.EventId = eve.Id;
            return Page();
        }

        public async Task<IActionResult> OnPostEventSettings()
        {
            if(ModelState.IsValid)
            {
                var result = await _eventManager.UpdateEvent(SettingsVM.EventName, SettingsVM.EventId, SettingsVM.SelectedEventImage);

                if(result)
                {
                    EventSettingMessage = "Event settings updated sucessfully.";
                }
                else
                {
                    ModelState.AddModelError("", "Error updating event settings");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostArchieveEvent()
        {
           var result = await _eventManager.ArchieveEvent(SettingsVM.EventId);

            if(result)
            {
                return RedirectToPage("/App/Events");
            }
            else
            {
                EventSettingMessage = "Couldnt archieve event";
            }

            return Page();
        }
    }
}