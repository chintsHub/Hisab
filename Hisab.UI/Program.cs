using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Common;
using Hisab.Common.Log;
using Hisab.UI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Hisab.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile("log-.txt",LogEventLevel.Debug)
                .CreateLogger();


           
            IWebHost webHost = CreateWebHostBuilder(args).Build();
            Log.Write(LogEventLevel.Information,"{@LogDetail}", LogHelper.CreateLogDetail(LogType.Diagnostic, "Sucessfully built WebHost", LogLayer.Server));

            try // seeding
            {
                var perfLog = LogHelper.CreatePerformanceLog("Seeding");
                using (var scope = webHost.Services.CreateScope())
                {

                    var seed = scope.ServiceProvider.GetRequiredService<IApplicationSeeding>();

                    await seed.CreateAdminUser();


                }
                perfLog.Stop();
                Log.Write(LogEventLevel.Information, "{@perfLog}", perfLog.LogDetail);
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error, "{@LogDetail}", LogHelper.CreateLogDetail(LogType.Error, "", ex: ex));


            }

                                 
            await webHost.RunAsync();

            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

       
    }
}
