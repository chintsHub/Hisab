using Hisab.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hisab.UI.Services
{
    public interface IFilterProcessor
    {
        JsonResult Process<T>(IQueryable<T> records, FilterOptions filterOptions);
    }

    public class FilterProcessor : IFilterProcessor
    {
        private SieveProcessor _sieveProcessor;
        public FilterProcessor(SieveProcessor SieveProcessor)
        {
            _sieveProcessor = SieveProcessor;
        }
        public JsonResult Process<T>(IQueryable<T> records, FilterOptions filterOptions)
        {
            SieveModel sieveModel = new SieveModel();

            //apply filter
            if (!String.IsNullOrEmpty(filterOptions.filter))
            {
                var str1 = filterOptions.filter.Replace(":\"", "@=");
                var str2 = str1.Replace(@"""", ""); // remove "
                var str3 = str2.Replace("\\", ""); // remove \
                var str4 = str3.Replace("}", "");
                sieveModel.Filters = str4.Replace("{", "");
                
            }
            // apply sort
            sieveModel.Sorts = filterOptions.Sort;
            if (filterOptions.Order == "desc")
                sieveModel.Sorts = "-" + sieveModel.Sorts;

            var filteredResult = _sieveProcessor.Apply(sieveModel, records);

            // apply Paging
            sieveModel.PageSize = filterOptions.Limit;
            if (filterOptions.Offset == 0)
                sieveModel.Page = 1;
            else
                sieveModel.Page = (int)(filterOptions.Offset / filterOptions.Limit) + 1;


            var finalResult = _sieveProcessor.Apply(sieveModel, filteredResult);

            //construct return object
            var jsonResult = new
            {
                total = filteredResult.Count(),
                totalNotFiltered = records.Count(),
                rows = finalResult.ToList()
            };


            return new JsonResult(jsonResult);
        }
    }
}
