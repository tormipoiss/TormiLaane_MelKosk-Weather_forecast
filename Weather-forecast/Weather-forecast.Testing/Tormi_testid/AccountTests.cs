using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Weather_forecast.Controllers;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Testing.Tormi_testid
{
    public class AccountTests : TestBase
    {
        [Fact]
        public void RegisterIndexGet()
        {
            AccountController accountController = MakeController();

            var result = accountController.Register();
            Assert.NotNull(result);
        }

        [Fact]
        public async void RegisterPostError_TakenUser()
        {
            AccountController accountController = MakeController();

            Account account = new Account
            {
                Username = "test",
                Password = "Abc-1",
                ConfirmPassword = "Abc-1"
            };

            await accountController.Register(account);
            var result = await accountController.Register(account);
            Assert.NotNull(result);
            Assert.Equal("This username is already taken., Please choose another one!", accountController.ModelState.ToString());
        }

        public AccountController MakeController()
        {
            var uid = "user123";
            var db = Svc<DatabaseContext>();
            db.SaveChanges();
            var userManager = MockUserManager(uid, new ApplicationUser { Id = uid });
            IHttpContextAccessor httpContext = new Mock<HttpContextAccessor>().Object;
            var identityOptions = new IdentityOptions();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            optionsMock.Setup(o => o.Value).Returns(identityOptions);
            IUserClaimsPrincipalFactory<ApplicationUser> UserClaimsPrincipalFactory = new Mock<UserClaimsPrincipalFactory<ApplicationUser>>(userManager, optionsMock.Object).Object;
            var signInManager = MockSignInManager(userManager, httpContext, UserClaimsPrincipalFactory, optionsMock.Object);
            AccountController accountController = new AccountController(
                userManager,
                signInManager,
                db
            );
            return accountController;
        }
    }
}