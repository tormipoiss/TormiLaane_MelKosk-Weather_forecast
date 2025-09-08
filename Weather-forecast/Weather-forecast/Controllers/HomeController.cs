using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherAPIHandler _weatherAPIHandler;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, WeatherAPIHandler weatherAPIHandler,DatabaseContext context)
        {
            _logger = logger;
            _weatherAPIHandler = weatherAPIHandler;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet("Home/SearchHistory")]
        public async Task<IActionResult> SearchHistory()
        {
            if (!Request.Cookies.TryGetValue("id", out var uid)) return BadRequest("No UUID!");
            var parsed = Guid.Parse(uid);
            var userHistory = _context.SearchHistory.FirstOrDefault(u => u.UserId == parsed);
            if (userHistory == null) return View(new History());
            var res = await _context.Cities.Where(cc=>cc.HistoryUserId == parsed).ToListAsync();
            return View(new History() { Cities = res,UserId = parsed});
        }

        [AllowAnonymous]
        [HttpPost("Home/City")]
        public async Task<IActionResult> SearchResults([FromForm] string? city)
        {
            if (string.IsNullOrEmpty(city)) return BadRequest("City can not be null!");
            if (!Request.Cookies.TryGetValue("id", out var uid)) return BadRequest("No UUID!");

            var parsed = Guid.Parse(uid);
            var userHistory = _context.SearchHistory.FirstOrDefault(u=>u.UserId == parsed);
            if (userHistory == null)
            {
                await _context.SearchHistory.AddAsync(
                    new() 
                    { 
                        UserId = parsed, 
                        Cities = new() {
                            new()
                            {
                                City = city,
                                Date = DateTime.Now
                            }
                        } 
                    });
            }
            else
            {
                userHistory.Cities.Add(new()
                {
                    City = city,
                    Date = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
            
            var result = await _weatherAPIHandler.FetchDataAsync(city);
            if (result == null) return BadRequest($"Failed to fetch weather data for {city}");
            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
