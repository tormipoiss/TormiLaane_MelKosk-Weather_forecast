using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class ShareLinkService
    {
        public ShareLinkService()
        {
            
        }
        public string CreateSharedLink(SharedLinkDto dto)
        {
            return $"https://localhost:5001/Home/Shared?city={dto.CityName}&shareToken={dto.ShareToken}&uid={dto.UserId}&metric={dto.IsMetric}&foreCastDate={dto.Date}&displayMultipleDays={dto.MultipleDays}&dayAmount={dto.DayAmount}";
        }
    }
}
