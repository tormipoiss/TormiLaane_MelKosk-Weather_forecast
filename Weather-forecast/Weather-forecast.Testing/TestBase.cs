using Microsoft.Extensions.DependencyInjection;
using Weather_forecast.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Weather_forecast.Testing.Macros;
using Microsoft.Extensions.Hosting;
using Weather_forecast.Testing.Mock;
using Microsoft.AspNetCore.Identity;
using Weather_forecast.Models;
using Moq;
using System.Security.Claims;

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
            //services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            services.AddDbContext<DatabaseContext>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }
        public static UserManager<ApplicationUser> MockUserManager(string id, ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            var mock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            mock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(id);
            mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            return mock.Object;
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
