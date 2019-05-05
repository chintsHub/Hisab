using System;
using System.Collections.Generic;

namespace Hisab.Common.Log
{

    public static class LogHelper
    {
        public static LogDetail CreateLogDetail(LogType logType, string message, LogLayer layer=LogLayer.Server, string location="", string username=null, Exception ex =null, string correlationId="")
        {
            return new LogDetail()
            {
                LogType = logType,
                Message = message,
                Layer = layer,
                Location = location,
                
                UserName = username ?? Environment.UserName,
                ElapseMilliseconds = null,
                Exception = ex,
                CorrelationId = correlationId,
                AdditionalInfo = null

            };
        }

        public static PerformanceLog CreatePerformanceLog(string name, string username="", string location="", LogLayer layer=LogLayer.Server)
        {
            return new PerformanceLog(name, username, location, layer);

        }
       
    }
}
