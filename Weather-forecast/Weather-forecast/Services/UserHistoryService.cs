using Weather_forecast.Data;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class UserHistoryService
    {
        private readonly DatabaseContext _context;
        public UserHistoryService(DatabaseContext context)
        {
            _context = context;
        }
        public List<City> GetUserHistoryAsList(string userId)
        {
            return _context.Cities.Where(City => City.HistoryUserId == userId).ToList();
        }
        public History? GetUserHistory(string userId)
        {
            return _context.SearchHistory.FirstOrDefault(History => History.UserId == userId);
        }
        public History AddUserHistory(History history)
        {
            _context.SearchHistory.Add(history);
            _context.SaveChanges();
            return history;
        }
        public History UpdateUserHistory(History history)
        {
            _context.SearchHistory.Update(history);
            _context.SaveChanges();
            return history;
        }
    }
}
