using Hisab.UI.ViewModels;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Services
{
    public class HisabCustomFilter : ISieveCustomFilterMethods
    {
        public IQueryable<FeedBackItemVm> feedbackTypeName(IQueryable<FeedBackItemVm> source, string op, string[] values)
        {
            return source;
        }
    }
}
