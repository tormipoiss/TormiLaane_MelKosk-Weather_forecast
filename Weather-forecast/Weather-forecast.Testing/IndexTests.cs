using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Weather_forecast.Controllers;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
namespace Weather_forecast.Testing
{
    public class IndexTests : TestBase
    {
        //[Fact]
        //public void Index_Should_Show_History_When_Exists()
        //{
        //    var uid = "user123";
        //    var db = Svc<DatabaseContext>();
        //    db.SearchHistory.Add(new History { UserId = uid });
        //    db.SaveChanges();
        //    var httpClient = new HttpClient();
        //    var mockWeatherHandler = new Moq.Mock<WeatherAPIHandler>(httpClient);
        //    var userManager = MockUserManager(uid, new ApplicationUser { Id = uid }); 
        //    var controller = new HomeController(
        //        Moq.Mock.Of<ILogger<HomeController>>(),
        //        mockWeatherHandler.Object,
        //        userManager,
        //        db
        //    );

        //    var result = controller.Index() as ViewResult;
        //    Assert.Equal("true", controller.ViewData["showHistory"]);
        //}
    }
}