using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory;
using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce
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
            string cs = Configuration.GetConnectionString("conStr");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(cs));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<IdentityUser, IdentityRole>().
                AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // for repository
            //services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddScoped<ICoverTypeRepoistory, CoverTypeRepository>();

            //2nd method
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                options.LogoutPath = $"/Identity/Account/Logout";
            });

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = "829879151609483";
                options.AppSecret = "7302ca0d82d064bf49fc21da6da81d1b";
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "50799036274-r62f0nde7mbj7io6mmou6lkml8qk7sod.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(30);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
            StripeConfiguration.ApiKey =
                Configuration.GetSection("Stripe")["SecretKey"];
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
