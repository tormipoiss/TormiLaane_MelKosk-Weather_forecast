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
using System.Globalization;

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
        public async Task<IActionResult> CityGet(CityAndApi model, string buttonType)
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
            var result = await _weatherAPIHandler.FetchDataAsync(model.City.CityName,model.Metric);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            if (model.Metric)
            {
                result.Units = new()
                {
                    KmOrMile = "km",
                    COrF = "C",
                    MmOrInches = "mm"
                };
            }
            else
            {
                result.Units = new()
                {
                    KmOrMile = "miles",
                    COrF = "F",
                    MmOrInches = "inches"
                };
            }
            result.Metric = model.Metric;
            result.ForecastDate = model.ForecastDate;
            var cityToHistory = new City();
            cityToHistory.CityName = model.City.CityName;
            cityToHistory.DateOfSearch = DateTime.Now;
            cityToHistory.HistoryUserId = uid;
            //ApplicationUser? usr = await _userManager.GetUserAsync(User);
            var alreadyShared = await _context.Shares.FirstOrDefaultAsync(x=>x.City==model.City.CityName);
            if (alreadyShared != null)
            {
                ViewBag.ShareLink = $"https://localhost:7089/Home/Shared?city={model.City.CityName}&shareToken={alreadyShared.ShareToken}&uid={uid}&metric={model.Metric}";
                ViewBag.ShareToken = alreadyShared.ShareToken;
            }
            else
            {
                string shareToken = Guid.NewGuid().ToString();
                ViewBag.ShareLink = $"https://localhost:7089/Home/Shared?city={model.City.CityName}&shareToken={shareToken}&uid={uid}&metric={model.Metric}";
                ViewBag.ShareToken = shareToken;
            }
            ViewBag.City = model.City.CityName;
            ViewBag.Uid = uid;
            ViewBag.Metric = model.Metric;
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
            if (buttonType == "MultipleDays")
            {
                result.DisplayMultipleDays = true;
                result.DayAmount = model.DayAmount;
            }
            else
            {
                result.DisplayMultipleDays = false;
            }
            return View("~/Views/Home/Index.cshtml", result);
        }

        [HttpGet("Home/GetForecastDetails")]
        [Authorize]
        public async Task<IActionResult> GetForecastDetails(string resolvedAddress, bool metric, string forecastDate)
        {
            var result = await _weatherAPIHandler.FetchDataAsync(resolvedAddress, metric);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (metric)
            {
                result.Units = new()
                {
                    KmOrMile = "km",
                    COrF = "C",
                    MmOrInches = "mm",
                    MOrFt = "m"
                };
            }
            else
            {
                result.Units = new()
                {
                    KmOrMile = "miles",
                    COrF = "F",
                    MmOrInches = "inches",
                    MOrFt = "ft"
                };
            }
            result.Metric = metric;
            try
            {
                result.ForecastDate = DateTime.Parse(forecastDate);
            }
            catch
            {
                result.ForecastDate = null;
            }

            return PartialView("~/Views/Home/GetForecastDetails.cshtml", result);
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
        [HttpGet("Home/DeleteSharedLink")] // Should be post but im too lazy to add a form to just delete a shared link
        [Authorize]
        public async Task<IActionResult> DeleteSharedLink(Guid shareToken)
        {
            if (!Request.Headers.TryGetValue("Referer",out var referer) || referer.Count != 1)
            {
                return RedirectToAction("Statistics");
            }
            if (!referer.First().EndsWith("/Home/Statistics"))
            {
                return RedirectToAction("Statistics");
            }
            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var share = await _context.Shares.FirstOrDefaultAsync(x => x.ShareToken == shareToken.ToString());
            if (share == null)
            {
                return RedirectToAction("Statistics");
            }
            if (share.UserId != uid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();
            return RedirectToAction("Statistics");
        }


        [HttpGet("Home/Shared")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForecastSharing(string city, Guid shareToken, Guid uid,bool metric)
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
            var result = await _weatherAPIHandler.FetchDataAsync(city,metric);
            ViewBag.SharedUrl = true;
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (metric)
            {
                result.Units = new()
                {
                    KmOrMile = "km",
                    COrF = "C",
                    MmOrInches = "mm"
                };
            }
            else
            {
                result.Units = new()
                {
                    KmOrMile = "miles",
                    COrF = "F",
                    MmOrInches = "inches"
                };
            }
            return View("~/Views/Home/Index.cshtml", result);
        }
        [HttpPost("Home/ShareLink")]
        [Authorize]
        public async Task<IActionResult> ConfirmShare(string city, Guid shareToken, Guid uid,bool metric)
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
            await _context.Shares.AddAsync(new() { City = city, ShareToken = shareToken.ToString(), UserId = uid.ToString(),Metric=metric});
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
