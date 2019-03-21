using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hisab.UI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hisab.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            

            //https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/

            

            using (var scope = webHost.Services.CreateScope())
            {
                var seed = scope.ServiceProvider.GetRequiredService<IApplicationSeeding>();

                await seed.CreateAdminUser();
            }

            await webHost.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
