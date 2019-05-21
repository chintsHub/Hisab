using System;
using System.Collections.Generic;
using System.Text;

namespace Hisab.Dapper.Model
{
    public class HisabEvent
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public int UserId { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

    }
}
