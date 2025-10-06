using Microsoft.AspNetCore.Identity;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Models
{
    public class ApplicationUser : IdentityUser
    {
        public History SearchHistory { get; set; }
        public List<CityShare>? ShareTokens { get; set; } = new();
        public bool GlobalMetric { get; set; } = true;
    }
}
