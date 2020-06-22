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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sieve.Models;
using Sieve.Services;

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true; //default value is false
                    
                })
                .AddDefaultTokenProviders()
                .AddSignInManager<HisabSignInManager<ApplicationUser>>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/App/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddNToastNotifyToastr();


            

            services.AddScoped<IDbConnectionProvider>(sp => new DbConnectionProvider(connectionString));

            services.AddScoped<IUserSettingManager>(sp => new UserSettingManager(sp.GetService<IDbConnectionProvider>()));

            services.AddScoped<IApplicationSeeding>(sp =>
                new ApplicationSeeding(sp.GetService<UserManager<ApplicationUser>>(), sp.GetService<IUserSettingManager>()));

            services.AddScoped<IEmailService>(sp => new EmailService(emailCredentials.AccessKey, emailCredentials.SecretKey));


            services.AddScoped<IEventManager>(sp => new EventManager(sp.GetService<IDbConnectionProvider>(), 
                sp.GetService<UserManager<ApplicationUser>>()));

            services.AddScoped<IEventInviteManager>(sp => new EventInviteManager(sp.GetService<IDbConnectionProvider>(),
                sp.GetService<UserManager<ApplicationUser>>(), sp.GetService<IEventManager>()));

            services.AddScoped<IFeedbackManager>(sp => new FeedbackManager(sp.GetService<IDbConnectionProvider>()));

            services.AddScoped<IEventJournalHelper>(sp => new EventJournalHelper(sp.GetService<IDbConnectionProvider>()));

            services.AddScoped<IEventTransactionManager>(sp => new EventTransactionManager(sp.GetService<IDbConnectionProvider>(), sp.GetService<IEventJournalHelper>()));

           

            services.AddScoped<ISieveCustomFilterMethods, HisabCustomFilter>();
            services.AddScoped<SieveProcessor>();
            services.AddScoped<IFilterProcessor, FilterProcessor>();
            
            services.Configure<SieveOptions>(_configuration.GetSection("Sieve"));



            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();

            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    app.UseHsts();
            //}

            app.UseExceptionHandler("/error");
            //app.UseHsts();
            //app.UseStatusCodePagesWithReExecute("/StatusCode", "?code={0}");

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
