using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string CityName { get; set; }
        public DateTime DateOfSearch { get; set; }
        public string HistoryUserId { get; set; }
    }
}
