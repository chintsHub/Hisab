using System.Net.Security;

namespace Hisab.Common.Log
{
    public enum LogType
    {
        Performance = 1,
        Usage,
        Error,
        Diagnostic
    }

    public enum LogLayer
    {
        Server=1,
        UI
    }
}
