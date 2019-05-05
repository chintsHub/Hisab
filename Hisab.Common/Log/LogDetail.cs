using System;
using System.Collections.Generic;

namespace Hisab.Common.Log
{
    public class LogDetail
    {
        public LogDetail()
        {
            Timestamp = DateTime.Now;
            Product = "Hisab";
            Hostname = Environment.MachineName;
        }

        public LogType LogType { get; set; }

        public DateTime Timestamp { get; private set; }

        public string Message { get; set; }

        //Where
        public string Product { get; private set; }

        public LogLayer Layer { get; set; }

        public string Location { get; set; }

        public string Hostname { get; private set; }

        //Who
        public string UserName { get; set; }

        //Everything Else
        public long? ElapseMilliseconds { get; set; } // only for performance entries
        public Exception Exception { get; set; } // error
        public string CorrelationId { get; set; } // exception shielding from server to client
        public Dictionary<string,object> AdditionalInfo { get; set; } 

    }
}
