using Hisab.Common.BO;
using Hisab.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Services
{
    public static class HisabImageManager
    {

        public static List<HisabImage> GetEventImages()
        {
            var retVal = new List<HisabImage>();

            retVal.Add(new HisabImage() { Id = 1, ImageName = "Beach holiday", ImagePath = "~/img/eventCardImage1.jpg" });
            retVal.Add(new HisabImage() { Id = 2, ImageName = "Restaurant", ImagePath = "~/img/eventCardImage2.jpg" });
            retVal.Add(new HisabImage() { Id = 3, ImageName = "Overseas trip", ImagePath = "~/img/eventCardOverseas1.jpg" });
            retVal.Add(new HisabImage() { Id = 4, ImageName = "Friends trip", ImagePath = "~/img/EventCardParty1.jpg" });
            retVal.Add(new HisabImage() { Id = 5, ImageName = "Road trip", ImagePath = "~/img/eventCardRoadTrip1.jpg" });
            retVal.Add(new HisabImage() { Id = 6, ImageName = "Sports trip", ImagePath = "~/img/eventCardSports1.jpg" });

            return retVal;
        }

        public static HisabImage GetRandomEventImage()
        {
            var rnd = new Random();
            var randImage = rnd.Next(1, 6);

            return GetEventImages().Where(x => x.Id == randImage).FirstOrDefault();
        }

        public static List<AvatarVm> GetAvatars()
        {
            var retVal = new List<AvatarVm>();

            retVal.Add(new AvatarVm() { Avatar = AvatarEnum.Default, AvatarImagePath = "~/img/img_avatar.png" });
            retVal.Add(new AvatarVm() { Avatar = AvatarEnum.Boy1, AvatarImagePath = "~/img/iconMale.png" });
            retVal.Add(new AvatarVm() { Avatar = AvatarEnum.Girl1, AvatarImagePath = "~/img/iconFemale.png" });
            retVal.Add(new AvatarVm() { Avatar = AvatarEnum.BoySuperhero1, AvatarImagePath = "~/img/iconMaleSuperhero.png" });
            retVal.Add(new AvatarVm() { Avatar = AvatarEnum.GirlSuperhero1, AvatarImagePath = "~/img/iconFemalSuperhero.png" });

            return retVal;
        }

        public static AvatarVm GetAvatar(AvatarEnum avatarEnum)
        {
            var avatars = GetAvatars();
            return avatars.Where(x => x.Avatar == avatarEnum).First();
        }
    }

    
}
