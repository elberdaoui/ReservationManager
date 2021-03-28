using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationManager.Areas.Identity.Data;
using ReservationManager.Models;

[assembly: HostingStartup(typeof(ReservationManager.Areas.Identity.IdentityHostingStartup))]
namespace ReservationManager.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<UserContext>(options =>
                    options.UseMySql(
                        context.Configuration.GetConnectionString("con")));

                services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                })
                    .AddEntityFrameworkStores<UserContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });
                services.AddAuthentication()
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddGoogle(options =>
                    {
                        IConfigurationSection googleAuthNSection =
                            context.Configuration.GetSection("Authentication:Google");

                        options.ClientId = googleAuthNSection["ClientId"];
                        options.ClientSecret = googleAuthNSection["ClientSecret"];
                        //options.CallbackPath = new PathString("/google");
                    });
            });
        }
    }
}