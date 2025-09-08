using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class CitySearch
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public Guid HistoryUserId { get; set; }
    }
}
