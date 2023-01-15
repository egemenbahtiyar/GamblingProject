using System;
using GamblingProject.Models;
using GamblingProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Moralis;
using Moralis.Models;

namespace GamblingProject
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
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSession();
            services.AddSingleton<UserService>();
            var mongoDbSettings = Configuration.GetSection(nameof(GamblingDatabaseSettings))
                .Get<GamblingDatabaseSettings>();
            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(mongoDbSettings.ConnectionString));
            services.AddIdentity<User, ApplicationRole>()
                .AddMongoDbStores<User, ApplicationRole, Guid>
                (
                    mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName
                );
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = ".42XBet.Security.Cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(90);
            });
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            // Setup Moralis
            MoralisClient.ConnectionData = new ServerConnectionData
            {
                ApiKey = "csRlJAc6TviBo1NSVMotruuhaX6D4ZLkEnSz236rgA8GOKMeBVupTkcF5Tj1P35n"
            };

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "authRequest",
                    "Authentication/RequestMessage/{address}/{network}/{chainId}",
                    new {controller = "Authentication", action = "RequestMessage"}
                );

                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}