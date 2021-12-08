using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodKept.Data;
using Microsoft.AspNetCore.Identity;
using FoodKept.Models;
using Serilog;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog.Formatting.Json;

namespace FoodKept
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(new JsonFormatter(), "Logs/log-.json", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSingleton(x => Log.Logger);
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

            services.AddDbContext<ShopContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ShopContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 1;
                }
                )
                //.AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ShopContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Login";
            });

            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ShopContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            services.AddScoped<IFoodRepository, SQLFoodRepository>();
            services.AddControllers();

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(x => Log.Logger).SingleInstance();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Base}/{action=Index}");
            });

        }
    }
}

