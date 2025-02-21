using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using IntranetPortal.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IntranetPortal.Configurations;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Services;
using Npgsql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore;

namespace IntranetPortal
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
            string connectionString = Configuration.GetConnectionString("PortalConnection");
            services.AddTransient<NpgsqlConnection>(e => new NpgsqlConnection(connectionString));
            services.AddAuthentication(SecurityConstants.ChxCookieAuthentication).AddCookie(SecurityConstants.ChxCookieAuthentication, options =>
            {
                options.Cookie.Name = SecurityConstants.ChxCookieAuthentication;
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Home/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

            services.AddControllersWithViews();


            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 268435456;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 268435456;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 268435456;
            });

            services.AddRazorPages();
            services.ConfigureServiceManagers();
            services.ConfigureRepositories();
            services.AddSingleton<DataProtectionEncryptionStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "WKS",
                areaName: "WKS",
                 pattern: "WKS/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "WSP",
                areaName: "WSP",
                 pattern: "WSP/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "AssetManager",
                areaName: "AssetManager",
                 pattern: "AssetManager/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Bams",
                areaName: "Bams",
                 pattern: "Bams/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "PartnerServices",
                areaName: "PartnerServices",
                 pattern: "PartnerServices/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "UserAdministration",
                areaName: "UserAdministration",
                 pattern: "UserAdministration/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "ContentManager",
                areaName: "ContentManager",
                 pattern: "ContentManager/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "LMS",
                areaName: "LMS",
                 pattern: "LMS/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                      name: "PMS",
                  areaName: "PMS",
                   pattern: "PMS/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "ERM",
                areaName: "ERM",
                 pattern: "ERM/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "CLM",
                areaName: "CLM",
                 pattern: "CLM/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "GlobalSettings",
                areaName: "GlobalSettings",
                 pattern: "GlobalSettings/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }
}
