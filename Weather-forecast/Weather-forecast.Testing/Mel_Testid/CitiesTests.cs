using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_forecast.Data;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class CitiesTests : TestBase
    {
        private async Task AddCitiesToHistory(string[] cities,string uid)
        {
            var db = Svc<DatabaseContext>();
            foreach(var city in cities)
            {
                await db.Cities.AddAsync(new() { CityName = city, HistoryUserId = uid});
            }
            db.SaveChanges();
        }
        [Fact]
        public async void TotalCities_ShouldBe_5()
        {
            string[] cities = new string[] { "Tallinn", "New York", "Paris", "Helsinki", "Pärnu" };
            string user = Guid.NewGuid().ToString();
            await AddCitiesToHistory(cities, user);
            var gottenCities = Svc<CitiesServices>().GetCitiesByUserID(user);
            Assert.Equal(cities.Length, gottenCities.Count);

            for (int i = 0; i < cities.Length; i++)
            {
                Assert.Equal(cities[i], gottenCities[i].CityName);
            }
        }
    }
}
