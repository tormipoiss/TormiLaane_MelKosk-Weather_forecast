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
    public class WeatherAPIHandlerTests : TestBase
    {
        [Fact]
        public async void Should_ReturnCorrectWeatherData_WhenGettingWeatherByLocation()
        {
            var result = await Svc<WeatherAPIHandler>().FetchDataAsync("Tallinn");

            Assert.Equal("Tallinn", result.Weather.address);
            Assert.Equal("https://maps.google.com/maps?q=59.4372+24.7453&t=k&z=15&ie=UTF8&iwloc=&output=embed", result.EmbedUrl);
            Assert.Equal("https://map.blitzortung.org/#11.8/59.4372/24.7453", result.LightningEmbedUrl);
            Assert.Equal("https://www.yr.no/en/content/59.4372,24.7453/meteogram.svg", result.MeteogramEmbedUrl);
            Assert.Equal("https://embed.ventusky.com/?p=59.4372;24.7453;7&l=temperature-2m", result.WeatherEmbedUrl);
            Assert.True(result.Metric);
            DateTime date = DateTime.Parse(result.Weather.currentConditions.datetime);
            Assert.True(date.ToString().Length > 5);
            Assert.True(result.Weather.days.Count == 15);
        }

        [Fact]
        public async void Should_ReturnCorrectWeatherData_WhenGettingWeatherByLocationAndNotMetric()
        {
            var result = await Svc<WeatherAPIHandler>().FetchDataAsync("Tallinn", false);

            Assert.Equal("Tallinn", result.Weather.address);
            Assert.False(result.Metric);
        }

        [Fact]
        public async void Should_ReturnNull_WhenGettingWeatherByBadLocation()
        {
            var result = await Svc<WeatherAPIHandler>().FetchDataAsync("jasdjiasdjsdajiasdjuasdjusdaujasdjuasduj");

            Assert.Null(result);
        }
    }
}