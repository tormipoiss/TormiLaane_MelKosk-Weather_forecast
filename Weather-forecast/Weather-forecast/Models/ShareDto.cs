using System.ComponentModel.DataAnnotations;

namespace Weather_forecast.Models
{
    public class ShareDto
    {
        public string? City { get; set; }
        public Guid? ShareToken { get; set; }
        public Guid? UID { get; set; }
        public DateTime? Date { get; set; }
        public bool Metric { get; set; }
    }
}
