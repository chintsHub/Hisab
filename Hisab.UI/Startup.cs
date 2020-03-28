using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisab.AWS;
using Hisab.BL;
using Hisab.Dapper;
using Hisab.Dapper.Identity;
using Hisab.Dapper.IdentityStores;
using Hisab.Dapper.Repository;
using Hisab.UI.Services;
using Hisab.UI.ViewModels;
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
            var emailCredentials = _configuration.GetSection("EmailServiceCredentials").Get<EmailServiceCredentials>();

            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true; //default value is false
                })
                .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromHours(24));

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home");

            services.AddMvc()
                .AddNToastNotifyToastr();



            services.AddScoped<IDbConnectionProvider>(sp => new DbConnectionProvider(connectionString));

            services.AddScoped<IApplicationSeeding>(sp =>
                new ApplicationSeeding(sp.GetService<UserManager<ApplicationUser>>()));

            services.AddScoped<IEmailService>(sp => new EmailService(emailCredentials.AccessKey, emailCredentials.SecretKey));


            services.AddScoped<IEventManager>(sp => new EventManager(sp.GetService<IDbConnectionProvider>(), 
                sp.GetService<UserManager<ApplicationUser>>()));

            services.AddScoped<IEventInviteManager>(sp => new EventInviteManager(sp.GetService<IDbConnectionProvider>()));

            services.AddScoped<IUserSettingManager>(sp => new UserSettingManager(sp.GetService<IDbConnectionProvider>()));


            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //app.UseExceptionHandler("/Error/500");
            //app.UseStatusCodePagesWithReExecute("/Error/{0}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseNodeModules(env);

            app.UseAuthentication();

            app.UseNToastNotify();


            app.UseMvc
            (options =>
            {
                options.MapRoute("Default", "/{controller}/{action}/{id?}",
                    new { controller = "Home", Action = "Index" });
            });





        }
    }
}
