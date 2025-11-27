using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather_forecast.Services;

namespace Weather_forecast.Testing.Mel_Testid
{
    public class WeatherAPIHandlerTests : TestBase
    {
        [Fact]
        public async void ShouldGet_CorrectWeatherData_Metric()
        {
            var gottenCities = await Svc<WeatherAPIHandler>().FetchDataAsync("Tallinn",metric:true);
            Assert.NotNull(gottenCities);
            Assert.Equal("Tallinn", gottenCities.Weather.resolvedAddress);
        }
    }
}
