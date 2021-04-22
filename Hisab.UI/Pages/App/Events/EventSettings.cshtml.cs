using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Hisab.UI
{
    [Authorize(Roles = "App User, Admin")]
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
            
            if(await _eventManager.CheckEventAccess(eve,User.Identity.Name))
            {
                SettingsVM.EventName = eve.EventName;
                SettingsVM.SelectedEventImage = eve.EventPicId;
                SettingsVM.EventId = eve.Id;
                SettingsVM.SelectedCurrency = eve.CurrencyCode;

                foreach (var cur in Currency.GetAll())
                {
                    SettingsVM.CountryCurrency.Add(new CurrencyVM() { Code = cur.Key, Name = cur.Value });
                }

                foreach (var f in eve.Friends)
                {
                    //only load event friends
                    if (f.EventFriendStatus == EventFriendStatus.EventFriend)
                    {
                        SettingsVM.Friends.Add(new EventFriendVm()
                        {
                            EventId = f.EventId,
                            UserId = f.UserId,
                            Email = f.Email,
                            Name = f.NickName,
                            Status = f.EventFriendStatus.GetDescription(),
                            EventFriendStatus = f.EventFriendStatus,
                            IsFriendActive = f.IsFriendActive,
                            Avatar = HisabImageManager.GetAvatar(f.Avatar)
                        });

                    }

                }
                this.ViewData.Add("EventTitle", eve.EventName);
                return Page();
            }

            throw new UnauthorizedAccessException();
        }

        

        public async Task<IActionResult> OnPostEventSettings()
        {
            if (ModelState.IsValid)
            {
                var settingsBo = new EventSettingsBO()
                {
                    EventId = SettingsVM.EventId,
                    EventName = SettingsVM.EventName,
                    SelectedEventImage = SettingsVM.SelectedEventImage,
                    SelectedCurrency = SettingsVM.SelectedCurrency

                };
                foreach(var f in SettingsVM.Friends)
                {
                   
                        settingsBo.Friends.Add(new EventFriendBO()
                        { UserId = f.UserId, EventId = SettingsVM.EventId, EventFriendStatus = f.EventFriendStatus, IsFriendActive = f.IsFriendActive });
                    
                        
                                      
                }



                var result = await _eventManager.UpdateEvenSettings(settingsBo);

                if (result)
                {
                    EventSettingMessage = "Event settings updated sucessfully.";
                    foreach (var cur in Currency.GetAll())
                    {
                        SettingsVM.CountryCurrency.Add(new CurrencyVM() { Code = cur.Key, Name = cur.Value });
                    }

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