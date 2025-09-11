using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherAPIHandler _weatherAPIHandler;

        public HomeController(ILogger<HomeController> logger, WeatherAPIHandler weatherAPIHandler)
        {
            _logger = logger;
            _weatherAPIHandler = weatherAPIHandler;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost("Home/City")]
        public async Task<IActionResult> SearchResults([FromForm] string? city)
        {
            if (string.IsNullOrEmpty(city)) return BadRequest("City can not be null!");
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
