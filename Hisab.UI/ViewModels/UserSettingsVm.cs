using Hisab.Common.BO;
using Hisab.UI.Extensions;
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
            Avatars = new List<AvatarVm>();
            Avatars.Add(new AvatarVm() { Avatar = AvatarEnum.Default, AvatarImagePath = "~/img/img_avatar.png" });
            Avatars.Add(new AvatarVm() { Avatar = AvatarEnum.Boy1, AvatarImagePath = "~/img/iconMale.png" });
            Avatars.Add(new AvatarVm() { Avatar = AvatarEnum.Girl1, AvatarImagePath = "~/img/iconFemale.png" });
            Avatars.Add(new AvatarVm() { Avatar = AvatarEnum.BoySuperhero1, AvatarImagePath = "~/img/iconMaleSuperhero.png" });
            Avatars.Add(new AvatarVm() { Avatar = AvatarEnum.GirlSuperhero1, AvatarImagePath = "~/img/iconFemalSuperhero.png" });
        }
    }
}
