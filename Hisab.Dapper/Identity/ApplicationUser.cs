using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Hisab.Dapper.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
      public  string NickName { get; set; }

       
    }
}
