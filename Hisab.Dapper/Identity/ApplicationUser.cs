using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Hisab.Dapper.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
      public  string NickName { get; set; }

      public int AvatarId { get; set; } 
    }
}
