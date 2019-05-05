using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Hisab.Common.Log
{
    public class PerformanceLog
    {
        private readonly Stopwatch _sw;
        private readonly LogDetail _logDetail;

        public PerformanceLog(string name, string username, string location, LogLayer layer)
        {
            _sw = Stopwatch.StartNew();

            _logDetail = new LogDetail();

            _logDetail.UserName = username;
            _logDetail.Message = name;
            _logDetail.LogType = LogType.Performance;
            _logDetail.Location = location;
            _logDetail.Layer = layer;

            var beginTime = DateTime.Now;
            _logDetail.AdditionalInfo = new Dictionary<string, object>()
            {
                {"Started", beginTime.ToString(CultureInfo.InvariantCulture)}
            };

         }

        public LogDetail LogDetail => _logDetail;

        public PerformanceLog(string name, string username, string location, LogLayer layer,
            Dictionary<string, object> parms):this(name,username,location,layer)
        {
            foreach (var parm in parms)
            {
                _logDetail.AdditionalInfo.Add("input-" + parm.Key,parm.Value);
            }
        }

        public void Stop()
        {
            _sw.Stop();
            _logDetail.ElapseMilliseconds = _sw.ElapsedMilliseconds;
        }
        
    }
    
}
