using Microsoft.AspNetCore.Identity;

namespace Weather_forecast.Models
{
    public class ApplicationUser : IdentityUser
    {
        public History SearchHistory { get; set; }
        public List<CityShare>? ShareTokens { get; set; } = new();
    }
}
