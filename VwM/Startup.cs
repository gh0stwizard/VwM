using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AutoMapper;
using VwM.Mappings;
using VwM.Hubs;
using VwM.BackgroundServices;
using VwM.CronTasks;
using VwM.ViewModels;

namespace VwM
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            const string CookiePrefix = "VwM";

            #region cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(90);
                    options.Cookie.Name = $".{CookiePrefix}.Authentication";
                    options.LoginPath = "/Login";
                    options.AccessDeniedPath = "/AccessDenied";
                });
            #endregion

            #region localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var defaultCulture = "en";
            var supportedCultures = new List<CultureInfo>{
                new CultureInfo(defaultCulture),
                new CultureInfo("ru")
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                var provider = new CookieRequestCultureProvider()
                {
                    CookieName = $".{CookiePrefix}.Culture"
                };
                options.RequestCultureProviders = new[] { provider };
            });
            #endregion

            #region mvc
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $".{CookiePrefix}.Antiforgery";
            });
            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson();
            #endregion

            #region automapper
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region authorization
            var vwmConfig = Configuration.GetSection("VwM");
            services.Configure<Authorization.Options>(vwmConfig.GetSection("Authorization"));
            services.AddSingleton<Authorization.IContext, Authorization.Context>();
            #endregion

            #region database
            var dbConfig = vwmConfig.GetSection("Database");
            services.Configure<Database.Models.DatabaseSettings>(dbConfig);
            services.AddSingleton<Database.Models.IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<Database.Models.DatabaseSettings>>().Value);
            services.AddSingleton<Database.Collections.DeviceCollection>();
            services.AddSingleton<Database.Collections.WhoisCollection>();
            services.AddSingleton<Database.Server.DatabaseStatus>();
            #endregion

            #region background services
            services.AddSingleton<PingRequestQueue>();
            services.AddSingleton<WhoisRequestQueue>();
            services.AddSingleton<IBackgroundTaskQueue<PingTaskQueue>, PingTaskQueue>();
            services.AddSingleton<IBackgroundTaskQueue<WhoisTaskQueue>, WhoisTaskQueue>();
            services.AddHostedService<PingHostedService>();
            services.AddHostedService<WhoisHostedService>();
            #endregion

            #region cron
            services.AddSingleton<ICronTask, CleanupPingCronTask>();
            services.AddSingleton<ICronTask, CleanupWhoisCronTask>();
            services.AddSingleton<ICronTask, DbStatusCronTask>();
            services.AddHostedService<CronService>();
            #endregion

            #region view models
            services.AddSingleton<ToolListViewModel>();
            #endregion

            #region signalr
            services.AddSignalR();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<HomeHub>("/hubs/home");
                endpoints.MapHub<PingHub>("/hubs/ping");
                endpoints.MapHub<WhoisHub>("/hubs/whois");

                endpoints.MapControllerRoute(
                    name: "shortcuts",
                    pattern: "{action}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
