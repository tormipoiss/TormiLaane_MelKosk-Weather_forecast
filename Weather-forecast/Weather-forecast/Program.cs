using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Microsoft.Extensions.DependencyInjection;
namespace Weather_forecast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
            builder.Services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("TestDB"));
            builder.Services.AddHttpClient<WeatherAPIHandler>();
            builder.Services.AddSingleton<QrCodeService>();
            builder.Services.AddSingleton<LocationByIPService>();
            builder.Services.AddTransient<SharesServices>();
            builder.Services.AddTransient<UserHistoryService>();
            builder.Services.AddTransient<ShareLinkService>();
            builder.Services.AddTransient<SaveDatabaseService>();
            builder.Services.AddTransient<UserService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews(option =>
            {
                option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 3;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("CustomEmailConfirmation")
    .AddDefaultUI();
            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var _  = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
