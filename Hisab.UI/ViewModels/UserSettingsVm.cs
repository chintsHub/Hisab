using Hisab.Common.BO;
using Hisab.UI.Extensions;
using Hisab.UI.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hisab.UI.ViewModels
{
    public class OldUserSettingsVm
    {
        public ChangePasswordVm ChangePasswordVm { get; set; }

        public UpdateUserSettingsVm UpdateNickNameVm { get; set; }
    }

    public class AvatarVm
    {
        public AvatarEnum Avatar { get; set; }

        public string AvatarImagePath { get; set; }

        
    }

    public class UserSettingsVm
    {
        [Required]
        public string UserName { get; set; }

        public List<AvatarVm> Avatars { get; }

        [Range(1, 5, ErrorMessage = "Please select an Avatar")]
        public int SelectedAvatarId { get; set; }

               

        public UserSettingsVm()
        {
            Avatars = HisabImageManager.GetAvatars();
           
        }
    }
}
