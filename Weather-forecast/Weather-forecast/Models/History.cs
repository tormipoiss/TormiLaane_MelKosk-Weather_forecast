using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class History
    {
        [Key]
        public string UserId { get; set; }
        public List<City>? Cities { get; set; } = new();
    }
}
