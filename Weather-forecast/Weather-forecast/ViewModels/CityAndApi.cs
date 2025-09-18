using Weather_forecast.Models;

namespace Weather_forecast.ViewModels
{
    public class CityAndApi
    {
        public WeatherforcecastAPIResponseModel Weather { get; set; }
        public City City { get; set; }
        public string EmbedUrl { get; set; }
        public string CityN { get; set; }
        public string ShareToken { get; set; }
        public string Uid { get; set; }
    }
}
