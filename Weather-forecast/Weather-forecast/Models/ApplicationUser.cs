using Microsoft.AspNetCore.Identity;

namespace Weather_forecast.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<SearchHistoryCity> SearchHistory { get; set; }
    }
}
