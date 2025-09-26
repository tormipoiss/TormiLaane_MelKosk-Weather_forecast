using System.ComponentModel.DataAnnotations;
using Weather_forecast.Models;

namespace Weather_forecast.ViewModels
{
    public class CityAndApi
    {
        public WeatherforcecastAPIResponseModel Weather { get; set; }
        public City City { get; set; }
        public List<City> Cities { get; set; }
        public Units Units { get; set; }
        public string EmbedUrl { get; set; }
        public string LightingEmbedUrl { get; set; }
        public bool Metric { get; set; } = true;
        public DateTime? ForecastDate { get; set; }
        public int? DayAmount { get; set; }
        public bool DisplayMultipleDays { get; set; }
    }
}
