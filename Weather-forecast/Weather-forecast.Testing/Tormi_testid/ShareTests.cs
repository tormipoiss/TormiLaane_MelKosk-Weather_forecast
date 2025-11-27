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
    public class ShareTests : TestBase
    {
        [Fact]
        public void Should_ReturnEmptyList_WhenGettingSharesByUserIDWithNoExistingShares()
        {
            var result = Svc<SharesServices>().GetUserShares(Guid.NewGuid().ToString());

            Assert.Empty(result);
        }

        [Fact]
        public async void Should_ReturnShares_WhenGettingSharesByUserIDWithExistingShares()
        {
            CityShare cityShare = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tallinn",
                ShareToken = "asjmasdjjnasdjnasdjnasdnasinasdsad",
                UserId = "1",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };

            await Svc<SharesServices>().AddAsync(cityShare);

            var result = Svc<SharesServices>().GetUserShares("1");

            Assert.Equal("Tallinn", result[0].City);
        }

        [Fact]
        public async void Should_ReturnNull_WhenGettingShareByShareTokenWithNoExistingShares()
        {
            var result = await Svc<SharesServices>().GetShareByShareToken(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async void Should_ReturnShares_WhenGettingSharesByShareTokenWithExistingShares()
        {
            Guid shareToken = Guid.NewGuid();

            CityShare cityShare = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tallinn",
                ShareToken = shareToken.ToString(),
                UserId = "1",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };

            await Svc<SharesServices>().AddAsync(cityShare);

            var result = await Svc<SharesServices>().GetShareByShareToken(shareToken);

            Assert.Equal("Tallinn", result.City);
        }

        [Fact]
        public async void Should_RemoveShare_WhenRemovingShareWithExistingShares()
        {
            Guid shareToken = Guid.NewGuid();
            Guid shareToken2 = Guid.NewGuid();

            CityShare cityShare = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tallinn",
                ShareToken = shareToken.ToString(),
                UserId = "1",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };
            CityShare cityShare2 = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tartu",
                ShareToken = shareToken2.ToString(),
                UserId = "2",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };

            await Svc<SharesServices>().AddAsync(cityShare);
            await Svc<SharesServices>().AddAsync(cityShare2);

            var result = await Svc<SharesServices>().GetShareByShareToken(shareToken2);

            Assert.Equal("Tartu", result.City);

            Svc<SharesServices>().Remove(cityShare2);

            var resultAfterRemove = await Svc<SharesServices>().GetShareByShareToken(shareToken2);

            Assert.Null(resultAfterRemove);
        }

        [Fact]
        public async void Should_ReturnNull_WhenGettingShareByShareLocationAndUserIDWithNoExistingShares()
        {
            var result = await Svc<SharesServices>().GetAlreadySharedShare("galbatrosso", "piim");

            Assert.Null(result);
        }

        [Fact]
        public async void Should_ReturnTartuLocationShare_WhenGettingShareByShareLocationAndUserIDWithExistingShares()
        {
            Guid shareToken = Guid.NewGuid();
            Guid shareToken2 = Guid.NewGuid();

            CityShare cityShare = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tallinn",
                ShareToken = shareToken.ToString(),
                UserId = "1",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };

            CityShare cityShare2 = new CityShare()
            {
                isMultipleDayForecast = true,
                City = "Tartu",
                ShareToken = shareToken2.ToString(),
                UserId = "1",
                Metric = true,
                forecastDate = new DateTime(2024, 11, 20, 10, 10, 50),
                DayAmount = 100,
                ViewCount = 100,
            };

            await Svc<SharesServices>().AddAsync(cityShare);
            await Svc<SharesServices>().AddAsync(cityShare2);

            var result = await Svc<SharesServices>().GetAlreadySharedShare("1", "Tartu");

            Assert.Equal("Tartu", result.City);
        }
    }
}