using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Hisab.Common.BO
{
    public static class Currency
    {
        private static IDictionary<string, string> currencyMap = new Dictionary<string, string>();
        static Currency()
        {
            //https://www.csharp-examples.net/culture-names/

            currencyMap.Add("USD", "US Dollar");
            currencyMap.Add("EUR", "Euro");
            currencyMap.Add("AUD", "Australian Dollar");
            currencyMap.Add("INR", "Indian Rupee");
            currencyMap.Add("CNY", "Yuan Renminbi");
            currencyMap.Add("NZD", "New Zealand Dollar");
            currencyMap.Add("FJD", "Fiji Dollar");
            currencyMap.Add("HKD", "Hong Kong Dollar");
            currencyMap.Add("GBP", "UK - Pound");
            currencyMap.Add("IDR", "Rupiah");
            currencyMap.Add("JPY", "Yen");
            currencyMap.Add("MYR", "Malaysian Ringgit");
            currencyMap.Add("MVR", "Rufiyaa");
            currencyMap.Add("MUR", "Mauritius Rupee");
            currencyMap.Add("MXN", "Mexican Peso");
            currencyMap.Add("NPR", "Nepalese Rupee");
            currencyMap.Add("PKR", "Pakistan Rupee");
            currencyMap.Add("SGD", "Singapore Dollar");
            currencyMap.Add("LKR", "Sri Lanka Rupee");
            currencyMap.Add("THB", "Baht");
        }

        public static IDictionary<string, string> GetAll()
        {
            return currencyMap;
        }

        

        public static string GetCurrencySymbolFromCode(string code, bool includeRegionName = true)
        {
            System.Globalization.RegionInfo regionInfo = (from culture in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures)
                                                          where culture.Name.Length > 0 && !culture.IsNeutralCulture
                                                          let region = new System.Globalization.RegionInfo(culture.Name)
                                                          where String.Equals(region.ISOCurrencySymbol, code, StringComparison.InvariantCultureIgnoreCase)
                                                          select region).First();
            
            if(includeRegionName)
                return regionInfo.TwoLetterISORegionName + regionInfo.CurrencySymbol;

            return regionInfo.CurrencySymbol;
        }
    }
}
