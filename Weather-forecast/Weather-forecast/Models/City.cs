using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("city name")]
        public string CityName { get; set; }
        public bool isMultipleDayForecast { get; set; }
        public int? DayAmount { get; set; }
        public DateTime DateOfSearch { get; set; }
        public DateTime? ForecastDate { get; set; }
        public string HistoryUserId { get; set; }
    }
}
