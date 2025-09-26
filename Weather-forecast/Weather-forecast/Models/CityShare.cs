using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class CityShare
    {
        [Key]
        public string ShareToken { get; set; }
        public string UserId { get; set; }
        public string City { get; set; }
        public bool Metric { get; set; }
        public int ViewCount { get; set; }
    }
}
