using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;

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
        public async Task<IActionResult> CityGet(CityAndApi model)
        {
            ViewBag.error = false;
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }
            //if (string.IsNullOrEmpty(city)) return BadRequest("City can not be null!");
            var result = await _weatherAPIHandler.FetchDataAsync(model.City.CityName);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            var cityToHistory = new City();
            cityToHistory.CityName = model.City.CityName;
            cityToHistory.DateOfSearch = DateTime.Now;
            //ApplicationUser? usr = await _userManager.GetUserAsync(User);
            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            cityToHistory.HistoryUserId = uid;
            History? testHistory = _context.SearchHistory.FirstOrDefault(History => History.UserId == uid);
            if (testHistory == default)
            {
                var history = new History();
                history.UserId = uid;
                _context.SearchHistory.Add(history);
                _context.SaveChanges();
            }
            History? oldHistory = _context.SearchHistory.First(History => History.UserId == uid);
            oldHistory.Cities.Add(cityToHistory);
            _context.SearchHistory.Update(oldHistory);
            _context.SaveChanges();
            return View("~/Views/Home/Index.cshtml", result);
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
