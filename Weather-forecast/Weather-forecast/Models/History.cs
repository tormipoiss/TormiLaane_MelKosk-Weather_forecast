using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Weather_forecast.Models
{
    public class History
    {
        [Key]
        public ulong Id{ get; set; }
        [Required]
        public string City { get; set; } = "";
        public DateTime DateAndTimeOfSearch { get; set; }
    }
}