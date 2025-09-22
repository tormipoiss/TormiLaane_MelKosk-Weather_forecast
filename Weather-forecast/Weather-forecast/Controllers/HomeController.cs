using System.Diagnostics;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var uid = _userManager.GetUserId(User);
            History? testHistory = _context.SearchHistory.FirstOrDefault(History => History.UserId == uid);
            if (testHistory != default)
            {
                ViewData["showHistory"] = "true";
            }
            return View();
        }

        [HttpPost("Home/City")]
        [Authorize]
        public async Task<IActionResult> CityGet(CityAndApi model)
        {
            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            List<City> testHistoryCityList = _context.Cities.Where(City => City.HistoryUserId == uid).ToList();
            if (testHistoryCityList.Count > 0)
            {
                ViewData["showHistory"] = "true";
            }
            ViewBag.error = false;
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }
            if (string.IsNullOrEmpty(model.City.CityName)) return BadRequest("City can not be null!");
            var result = await _weatherAPIHandler.FetchDataAsync(model.City.CityName);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            result.ForecastDate = model.ForecastDate;
            var cityToHistory = new City();
            cityToHistory.CityName = model.City.CityName;
            cityToHistory.DateOfSearch = DateTime.Now;
            cityToHistory.HistoryUserId = uid;
            //ApplicationUser? usr = await _userManager.GetUserAsync(User);
            var alreadyShared = await _context.Shares.FirstOrDefaultAsync(x=>x.City==model.City.CityName);
            if (alreadyShared != null)
            {
                ViewBag.ShareLink = $"https://localhost:7089/Home/Shared?city={model.City.CityName}&shareToken={alreadyShared.ShareToken}&uid={uid}";
                ViewBag.Uid = uid;
                ViewBag.ShareToken = alreadyShared.ShareToken;
                ViewBag.City = model.City.CityName;
            }
            else
            {
                string shareToken = Guid.NewGuid().ToString();
                ViewBag.ShareLink = $"https://localhost:7089/Home/Shared?city={model.City.CityName}&shareToken={shareToken}&uid={uid}";
                ViewBag.Uid = uid;
                ViewBag.ShareToken = shareToken;
                ViewBag.City = model.City.CityName;
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

        [HttpGet("Home/History")]
        [Authorize]
        public async Task<IActionResult> History()
        {
            var uid = _userManager.GetUserId(User);
            CityAndApi historyCities = new CityAndApi();
            historyCities.Cities = _context.Cities.Where(City => City.HistoryUserId == uid).ToList();
            return View(historyCities);
        }
        [HttpGet("Home/Statistics")]
        [Authorize]
        public IActionResult ShareLinkStatistics()
        {
            var uid = _userManager.GetUserId(User);
            if(uid == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var allShares = _context.Shares.Where(x=>x.UserId==uid).ToList();
            if(allShares.Count == 0)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            return View(new LinkShares() { Shares=allShares});
        }


        [HttpGet("Home/Shared")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForecastSharing(string city, Guid shareToken, Guid uid)
        {
            if (city == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var exists = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (exists == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            if (exists.City != city)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var usr = await _context.Users.FirstOrDefaultAsync(x => x.Id == uid.ToString());
            if (usr == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            if(User.Identity != null && !User.Identity.IsAuthenticated)
            {
                exists.ViewCount++;
            }
            await _context.SaveChangesAsync();

            var result = await _weatherAPIHandler.FetchDataAsync(city);
            ViewBag.SharedUrl = true;
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            return View("~/Views/Home/Index.cshtml", result);
        }
        [HttpPost("Home/ShareLink")]
        [Authorize]
        public async Task<IActionResult> ConfirmShare(string city, Guid shareToken, Guid uid)
        {
            var exists = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (exists != null)
            {
                return BadRequest();
            }
            var userExistsId = _userManager.GetUserId(User);
            if (userExistsId == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (userExistsId != uid.ToString())
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            await _context.Shares.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString() });
            await _context.SaveChangesAsync();
            //var existingUserByName = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok();
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
