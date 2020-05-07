using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.BL;
using Hisab.Dapper.Identity;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hisab.UI.Pages.App.Admin
{
    public class AllUsersModel : PageModel
    {
        [BindProperty]
        public ApplicationUserVm UserVm { get; set; }

        private IFilterProcessor _filterProcessor;
        private UserManager<ApplicationUser> _userManager;
        private IUserSettingManager _userSettingManager;

        public AllUsersModel(IFilterProcessor filterProcessor, 
            UserManager<ApplicationUser> userManager,
            IUserSettingManager userSettingManager)
        {
            _filterProcessor = filterProcessor;
            _userManager = userManager;
            _userSettingManager = userSettingManager;
        }
        public void OnGet()
        {

        }

        public JsonResult OnGetLoadData(FilterOptions model)
        {

            var userList = _userManager.Users;

            var returnValue = _filterProcessor.Process(GetUserVM(userList), model);




            return new JsonResult(returnValue.Value);
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByIdAsync(UserVm.Id.ToString());

            if(user != null)
            {
                var result = await _userSettingManager.UpdateUserSettings(UserVm.NickName, UserVm.Id, UserVm.IsUserActive, UserVm.EmailConfirmed);
                return new OkResult();
            }

            
            return new BadRequestResult();
        }

        private IQueryable<ApplicationUserVm> GetUserVM(IQueryable<ApplicationUser> users)
        {
            var ApplicationUserVm = new List<ApplicationUserVm>();

            foreach (var u in users.ToList())
            {
                ApplicationUserVm.Add(new ViewModels.ApplicationUserVm
                {
                    Id = u.Id,
                    EmailConfirmed = u.EmailConfirmed,
                    NickName = u.NickName,
                    UserName = u.UserName,
                    IsUserActive = u.IsUserActive

                });
            }

            return ApplicationUserVm.AsQueryable();
        }
    }
}