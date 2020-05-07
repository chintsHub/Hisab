using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.ViewModels
{
     public class FilterOptions
    {
        public string Sort { get; set; }

        public string Order { get; set; }

        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string filter { get; set; }

        private SieveModel _sieveModel;

        public FilterOptions()
        {
            _sieveModel = new SieveModel();
        }

        
    }
}
