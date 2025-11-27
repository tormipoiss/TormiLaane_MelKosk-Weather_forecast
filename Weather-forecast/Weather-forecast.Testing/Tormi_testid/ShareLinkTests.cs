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
    public class ShareLinkTests : TestBase
    {
        [Fact]
        public void Should_CreateSharedLink_WhenUsingSharedLinkDto()
        {
            SharedLinkDto sharedLinkDto = new SharedLinkDto()
            {
                MultipleDays = true,
                CityName = "Tallinn",
                ShareToken = "asjmasdjjnasdjnasdjnasdnasinasdsad",
                UserId = "asdhjdasniasdinsadinasdnsadunasdun",
                IsMetric = true,
                Date = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100
            };

            var result = Svc<ShareLinkService>().CreateSharedLink(sharedLinkDto);

            Assert.Equal("https://localhost:5001/Home/Shared?city=Tallinn&shareToken=asjmasdjjnasdjnasdjnasdnasinasdsad&uid=asdhjdasniasdinsadinasdnsadunasdun&metric=True&foreCastDate=20.11.2024 10:10:50&displayMultipleDays=True&dayAmount=100", result);
        }
    }
}