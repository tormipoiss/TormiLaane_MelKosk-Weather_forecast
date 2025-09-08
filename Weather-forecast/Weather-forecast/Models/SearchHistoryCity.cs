using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class SearchHistoryCity
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime DateOfSearch { get; set; }
        public Guid HistoryUserId { get; set; }
    }
}
