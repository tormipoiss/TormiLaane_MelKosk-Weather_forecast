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
            //services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            services.AddDbContext<DatabaseContext>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(TUser user) where TUser : class
        {
            //string password = null;
            //var store = new Mock<IUserStore<ApplicationUser>>();

            //var mock = new Mock<UserManager<ApplicationUser>>(
            //    store.Object, null, null, null, null, null, null, null, null);

            //mock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(id);
            //mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            //mock.Setup(m => m.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);

            //return mock.Object;

            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<TUser>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            //mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            //mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return userManager;
        }
        public static UserManager<ApplicationUser> MockUserManager(string id, ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            var mock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            //mock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(id);
            //mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            //mock.Setup(m => m.CreateAsync(user, "Abc-1")).ReturnsAsync(IdentityResult.Success);
            //mock.Setup(m => m.CreateAsync(A<ApplicationUser>, "Abc-1").ReturnsAsync(IdentityResult.Success);

            return mock.Object;
        }
        public static SignInManager<ApplicationUser> MockSignInManager(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> UserClaimsPrincipalFactory,
            IOptions<IdentityOptions> configMock)
        {
            //var store = new Mock<IUserStore<ApplicationUser>>();

            var mock = new Mock<SignInManager<ApplicationUser>>(
                userManager, contextAccessor, UserClaimsPrincipalFactory, configMock, null, null, null);

            //mock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(id);
            //mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

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
