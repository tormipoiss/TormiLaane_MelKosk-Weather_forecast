using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Weather_forecast.Controllers;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.Testing.Macros;
using Weather_forecast.Testing.Mock;

namespace Weather_forecast.Testing
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }
        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);
            serviceProvider = services.BuildServiceProvider();
        }
        public virtual void SetupServices(IServiceCollection services)
        {
            services.AddScoped<LocationByIPService>();
            services.AddScoped<QrCodeService>();
            services.AddScoped<UserHistoryService>();
            services.AddScoped<SharesServices>();
            services.AddScoped<ShareLinkService>();
            services.AddScoped<UserService>();
            services.AddHttpClient<WeatherAPIHandler>();

            services.AddDbContext<DatabaseContext>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }
        public void Dispose() {}

        protected T Svc<T>()
        {
            return serviceProvider.GetService<T>();
        }

        private void RegisterMacros(IServiceCollection services)
        {
            var macroBaseType = typeof(IMacros);
            var macros = macroBaseType.Assembly.GetTypes().Where(t => macroBaseType.IsAssignableFrom(t)
            && !t.IsInterface && !t.IsAbstract);

            foreach (var macro in macros) { services.AddSingleton(macro); }
        }
    }
}
