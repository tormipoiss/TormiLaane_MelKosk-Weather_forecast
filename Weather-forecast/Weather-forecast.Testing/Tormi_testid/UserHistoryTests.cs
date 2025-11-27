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
    public class UserHistoryTests : TestBase
    {
        [Fact]
        public void Should_ReturnEmptyList_WhenGettingCitiesByUserIDWithNoUserHistory()
        {
            var result = Svc<UserHistoryService>().GetUserHistoryAsList(Guid.NewGuid().ToString());

            Assert.Empty(result);
        }

        [Fact]
        public void Should_ReturnCities_WhenGettingCitiesByUserIDWithExistingUserHistory()
        {
            List<City> Cities = new List<City>();

            City city = new City()
            {
                CityName = "Test"
            };

            City city2 = new City()
            {
                CityName = "Test"
            };

            Cities.Add(city);
            Cities.Add(city2);

            History history = new History()
            {
                UserId = "2",
                Cities = Cities
            };

            Svc<UserHistoryService>().AddUserHistory(history);

            var result = Svc<UserHistoryService>().GetUserHistoryAsList("2");

            Assert.Equal("Test", result[0].CityName);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Should_ReturnNull_WhenGettingHistoryByUserIDWithNoUserHistory()
        {
            var result = Svc<UserHistoryService>().GetUserHistory(Guid.NewGuid().ToString());

            Assert.Null(result);
        }

        [Fact]
        public void Should_ReturnHistory_WhenGettingHistoryByUserIDWithExistingUserHistory()
        {
            List<City> Cities = new List<City>();

            City city = new City()
            {
                CityName = "Test"
            };

            City city2 = new City()
            {
                CityName = "Test"
            };

            Cities.Add(city);
            Cities.Add(city2);

            History history = new History()
            {
                UserId = "3",
                Cities = Cities
            };

            Svc<UserHistoryService>().AddUserHistory(history);

            var result = Svc<UserHistoryService>().GetUserHistory("3");

            Assert.Equal("Test", result.Cities[0].CityName);
        }

        [Fact]
        public void Should_ReturnUpdatedHistory_WhenGettingHistoryByUserIDWithExistingUpdatedUserHistory()
        {
            List<City> Cities = new List<City>();

            City city = new City()
            {
                Id = 1,
                CityName = "Test"
            };

            City city2 = new City()
            {
                Id = 2,
                CityName = "Test"
            };

            Cities.Add(city);
            Cities.Add(city2);

            History history = new History()
            {
                UserId = "1",
                Cities = Cities
            };

            Svc<UserHistoryService>().AddUserHistory(history);

            var result = Svc<UserHistoryService>().GetUserHistory("1");

            Assert.Equal("Test", result.Cities[0].CityName);

            history.Cities[0].CityName = "UusTest";

            Svc<UserHistoryService>().UpdateUserHistory(history);

            var resultUpdated = Svc<UserHistoryService>().GetUserHistory("1");

            Assert.Equal("UusTest", resultUpdated.Cities[0].CityName);
        }
    }
}