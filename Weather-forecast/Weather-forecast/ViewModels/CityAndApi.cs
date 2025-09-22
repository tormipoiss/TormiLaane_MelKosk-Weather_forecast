using Weather_forecast.Models;

namespace Weather_forecast.ViewModels
{
    public class CityAndApi
    {
        public WeatherforcecastAPIResponseModel Weather { get; set; }
        public City City { get; set; }
        public List<City> Cities { get; set; }
        public string EmbedUrl { get; set; }
        public string LightingEmbedUrl { get; set; }
    }
}
