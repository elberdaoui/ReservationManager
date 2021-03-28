using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using ReservationManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationManager
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
            services.AddRazorPages();
            services.AddMvc();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNToastNotifyNoty(new NToastNotify.NotyOptions()
                {
                    ProgressBar = true, //Showing progressbar
                    //PositionClass = ToastPositions.TopRight,
                    Timeout = 4000, //time the notification takes to disappear (ms)
                    Theme = "mint" //theme name Notify.Js
                });
            services.AddDbContext<ReservationContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("con")));
            //services.AddAuthentication()
            //        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddGoogle(options =>
            //        {
            //            IConfigurationSection googleAuthNSection =
            //                Configuration.GetSection("Authentication:Google");

            //            options.ClientId = googleAuthNSection["ClientId"];
            //            options.ClientSecret = googleAuthNSection["ClientSecret"];
            //            //options.ClientId = "184982332345-uaj3fvoplf0fifiaq93i8bertaih3f7v.apps.googleusercontent.com";
            //            //options.ClientSecret = "JP0DR6ASrn4esyH7vHtxqJt8";
            //        });
            
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseNToastNotify();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Reservation}/{action=Index}/{id?}");
            });
        }
    }
}
