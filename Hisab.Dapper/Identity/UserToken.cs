using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Dapper.Identity
{
    public class UserToken
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
