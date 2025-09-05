using Microsoft.EntityFrameworkCore;
using Weather_forecast.Data;
using Weather_forecast.Services;

namespace Weather_forecast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
            builder.Services.AddHttpClient<WeatherAPIHandler>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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
            app.UseHttpsRedirection();
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
