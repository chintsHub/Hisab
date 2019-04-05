using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.Dapper.IdentityStores;
using Hisab.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hisab.UI
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("hisabDb");

            services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = false; //default value is false
                })
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home");

            services.AddMvc();
  
              

            
            services.AddScoped<IDbConnectionProvider>(sp => new DbConnectionProvider(connectionString));

            services.AddScoped<IApplicationSeeding>(sp =>
                new ApplicationSeeding(sp.GetService<UserManager<ApplicationUser>>()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseNodeModules(env);

            app.UseAuthentication();

            app.UseMvc(options =>
                {
                    options.MapRoute("Default", "/{controller}/{action}/{id?}", 
                        new { controller = "Home", Action = "Index" });
                });



          

        }
    }
}
