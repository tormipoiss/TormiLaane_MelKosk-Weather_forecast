using Weather_forecast.Models;

namespace Weather_forecast.ViewModels
{
    public class CityAndApi
    {
        public WeatherforcecastAPIResponseModel Weather { get; set; }
        public City City { get; set; }
    }
}
