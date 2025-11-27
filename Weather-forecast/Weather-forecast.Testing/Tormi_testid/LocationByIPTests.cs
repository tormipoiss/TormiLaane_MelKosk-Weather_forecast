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
    public class LocationByIPTests : TestBase
    {
        [Fact]
        public async void Should_ReturnTallinn_WhenGettingLocationByIP()
        {
            var result = await Svc<LocationByIPService>().LocationByIP();

            Assert.Equal("Tallinn", result);
        }
    }
}