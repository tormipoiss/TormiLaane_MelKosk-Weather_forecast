using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherAPIHandler _weatherAPIHandler;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, WeatherAPIHandler weatherAPIHandler, UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _logger = logger;
            _weatherAPIHandler = weatherAPIHandler;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Home/City")]
        [Authorize]
        public async Task<IActionResult> Index([FromForm] string? city)
        {
            if (string.IsNullOrEmpty(city)) return BadRequest("City can not be null!");
            var result = await _weatherAPIHandler.FetchDataAsync(city);
            if (result == null) return BadRequest($"Failed to fetch weather data for {city}");
            var cityToHistory = new City();
            cityToHistory.CityName = city;
            cityToHistory.DateOfSearch = DateTime.Now;
            ApplicationUser usr = await _userManager.GetUserAsync(HttpContext.User);
            cityToHistory.HistoryUserId = usr?.Id;
            if (!_context.SearchHistory.Any())
            {
                var history = new History();
                history.UserId = usr?.Id;
                _context.SearchHistory.Add(history);
            }
            History oldHistory = (History)_context.SearchHistory.Where(History => History.UserId == usr.Id);
            oldHistory.Cities.Add(cityToHistory);
            _context.SearchHistory.Update(oldHistory);
            _context.SaveChanges();
            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
