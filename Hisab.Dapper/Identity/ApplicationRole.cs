using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Hisab.Dapper.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string NormalizedName { get; set; }
        //public string ConcurrencyStamp { get; set; }
        //internal List<Claim> Claims { get; set; }
    }
}
