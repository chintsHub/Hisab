using Hisab.Common.BO;
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
    }

    
}
