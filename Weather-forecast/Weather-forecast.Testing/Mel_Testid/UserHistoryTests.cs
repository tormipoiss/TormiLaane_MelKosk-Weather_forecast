using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class UserHistoryTests : TestBase
    {
        private async Task AddCitiesToHistory(string[] cities, string uid)
        {
            var db = Svc<DatabaseContext>();
            foreach (var city in cities)
            {
                await db.Cities.AddAsync(new() { CityName = city, HistoryUserId = uid });
            }
            db.SaveChanges();
        }
        [Fact]
        public async void TotalCities_ShouldBe_5()
        {
            string[] cities = new string[] { "Tallinn", "New York", "Paris", "Helsinki", "Pärnu" };
            string user = Guid.NewGuid().ToString();
            await AddCitiesToHistory(cities, user);
            var gottenCities = Svc<UserHistoryService>().GetUserHistoryAsList(user);
            Assert.Equal(cities.Length, gottenCities.Count);

            for (int i = 0; i < cities.Length; i++)
            {
                Assert.Equal(cities[i], gottenCities[i].CityName);
            }
        }
        [Fact]
        public async void AddUserHistory_And_GetUserHistory_Success()
        {
            string[] cities = new string[] { "Tallinn", "New York", "Paris", "Helsinki", "Pärnu" };
            var service = Svc<UserHistoryService>();
            string user = Guid.NewGuid().ToString();
            History history = new()
            {
                UserId = user,
                Cities = new()
            };
            foreach (var c in cities)
            {
                history.Cities.Add(new() { CityName = c });
            }
            service.AddUserHistory(history);
            var gottenCities = Svc<UserHistoryService>().GetUserHistory(user);
            Assert.Equal(cities.Length, gottenCities.Cities.Count);
        }
        [Fact]
        public async void UpdateUserHistory_Success()
        {
            string[] cities = new string[] { "Tallinn", "New York", "Paris", "Helsinki", "Pärnu" };
            var service = Svc<UserHistoryService>();
            string user = Guid.NewGuid().ToString();
            History history = new()
            {
                UserId = user,
                Cities = new()
            };
            foreach (var c in cities)
            {
                history.Cities.Add(new() { CityName = c });
            }
            service.AddUserHistory(history);
            var gottenCities = Svc<UserHistoryService>().GetUserHistory(user);
            Assert.Equal(cities.Length, gottenCities.Cities.Count);
            history.Cities.Clear();
            service.UpdateUserHistory(history);
            var gottenCities2 = Svc<UserHistoryService>().GetUserHistory(user);
            Assert.Equal(0, gottenCities2.Cities.Count);
        }
    }
}
