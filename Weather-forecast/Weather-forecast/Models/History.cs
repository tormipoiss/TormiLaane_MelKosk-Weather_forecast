using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Weather_forecast.Models
{
    public class History
    {
        [Key]
        public Guid UserId{ get; set; }
        public List<CitySearch> Cities { get; set; } = new();
    }
}
