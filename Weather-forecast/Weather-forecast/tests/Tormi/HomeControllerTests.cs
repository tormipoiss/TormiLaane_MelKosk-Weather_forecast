using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Weather_forecast.Controllers;
using Weather_forecast.Data;
using Weather_forecast.Services;
using Weather_forecast.Models;

namespace Weather_forecast.Tests.Tormi;

public class HomeControllerTests
{
    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private static ClaimsPrincipal BuildPrincipal(string userId = "uid1")
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task GetForecastDetails_UserNull_ReturnsIndexWithError()
    {
        // Arrange
        var userMgr = MockUserManager();
        userMgr.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);

        var api = new Mock<WeatherAPIHandler>(); // or concrete WeatherAPIHandler if your constructor requires it
        var logger = new Mock<ILogger<HomeController>>().Object;

        var dbOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        var dbContext = new DatabaseContext(dbOptions);

        var controller = new HomeController(logger, api.Object, userMgr.Object, dbContext);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = BuildPrincipal() }
        };

        // Act
        var result = await controller.GetForecastDetails("Tallinn,EE", "2025-10-30");

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("~/Views/Home/Index.cshtml", view.ViewName);
        Assert.True(controller.ViewBag.error);
    }
}