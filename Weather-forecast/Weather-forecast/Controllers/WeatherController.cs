using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Json;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;
using Weather_forecast.ViewModels;
using static QRCoder.PayloadGenerator;

namespace Weather_forecast.Controllers
{
    public class WeatherController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherAPIHandler _weatherAPIHandler;
        private readonly QrCodeService _qrCodeService;
        private readonly LocationByIPService _locationByIPService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserHistoryService _historyService;
        private readonly ShareLinkService _shareLinkService;

        public WeatherController(ILogger<HomeController> logger, WeatherAPIHandler weatherAPIHandler, UserManager<ApplicationUser> userManager, DatabaseContext context, QrCodeService qrCodeService, LocationByIPService locationByIPService, UserHistoryService historyService, ShareLinkService shareLinkService)
        {
            _logger = logger;
            _weatherAPIHandler = weatherAPIHandler;
            _userManager = userManager;
            _context = context;
            _qrCodeService = qrCodeService;
            _locationByIPService = locationByIPService;
            _historyService = historyService;
            _shareLinkService = shareLinkService;
        }

        [HttpPost("Home/City")]
        [Authorize]
        public async Task<IActionResult> CityGet(CityAndApi model, string buttonType)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml", model);
            }

            if (string.IsNullOrEmpty(model.City.CityName))
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }

            var uid = _userManager.GetUserId(User);
            if (uid == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            var userHistory = _historyService.GetUserHistory(uid);
            if (userHistory != null)
            {
                ViewData["showHistory"] = "true";
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }
            if (buttonType == "GPS")
            {
                string LocationByIP = await _locationByIPService.LocationByIP();

                ViewBag.ShowModelError = false;

                model.City.CityName = LocationByIP;
                return View("~/Views/Home/Index.cshtml", model);
            }
            ViewBag.ShowModelError = true;
            ViewBag.error = false;
            var result = await _weatherAPIHandler.FetchDataAsync(model.City.CityName, user.GlobalMetric);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml", model);
            }

            if (user.GlobalMetric)
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
            result.ForecastDate = model.ForecastDate != null ? model.ForecastDate : model.City.ForecastDate != null ? model.City.ForecastDate : DateTime.Now;
            var cityToHistory = new City();
            cityToHistory.CityName = model.City.CityName;
            cityToHistory.DateOfSearch = DateTime.Now;
            cityToHistory.HistoryUserId = uid;
            cityToHistory.ForecastDate = model.ForecastDate;
            var alreadyShared = await _context.Shares.Where(u => u.UserId == uid).FirstOrDefaultAsync(x => x.City == model.City.CityName);
            if (buttonType == "MultipleDays" || model.City.isMultipleDayForecast == true)
            {
                result.DisplayMultipleDays = true;
                cityToHistory.isMultipleDayForecast = true;
                cityToHistory.DayAmount = model.City.DayAmount != null ? model.City.DayAmount : model.DayAmount;
                result.DayAmount = model.City.DayAmount != null ? model.City.DayAmount : model.DayAmount;
                ViewBag.DisplayMultipleDays = "true";
                ViewBag.DayAmount = model.City.DayAmount != null ? model.City.DayAmount : model.DayAmount;
            }
            else
            {
                cityToHistory.isMultipleDayForecast = false;
                result.DisplayMultipleDays = false;
                ViewBag.DisplayMultipleDays = "false";
                ViewBag.DayAmount = null;
            }
            //ApplicationUser? usr = await _userManager.GetUserAsync(User);
            string shareToken = alreadyShared != null ? alreadyShared.ShareToken : Guid.NewGuid().ToString();
            ViewBag.ShareLink = _shareLinkService.CreateSharedLink(
                    new()
                    {
                        CityName = model.City.CityName,
                        Date = result.ForecastDate,
                        ShareToken = shareToken,
                        UserId = uid,
                        IsMetric = model.Metric,
                        MultipleDays = (buttonType == "MultipleDays" || model.City.isMultipleDayForecast == true) ? true : false,
                        DayAmount = (model.City.DayAmount != null ? model.City.DayAmount : model.DayAmount)
                    });
            ViewBag.ShareToken = shareToken;
            ViewBag.City = model.City.CityName;
            ViewBag.Uid = uid;
            ViewBag.Metric = model.Metric;
            ViewBag.ForecastDate = model.ForecastDate != null ? model.ForecastDate : model.City.ForecastDate != null ? model.City.ForecastDate : DateTime.Now;
            result.QrCodeBase64 = _qrCodeService.CreateSharedForecastQRCodeAsB64(ViewBag.ShareLink);


            cityToHistory.HistoryUserId = uid;

            if (userHistory == null)
            {
                userHistory = _historyService.AddUserHistory(new() { UserId = uid });
            }
            userHistory.Cities.Add(cityToHistory);
            _historyService.UpdateUserHistory(userHistory);
            ViewData["showHistory"] = "true";

            return View("~/Views/Home/Index.cshtml", result);
        }

        [HttpGet("Home/GetForecastDetails")]
        [Authorize]
        public async Task<IActionResult> GetForecastDetails(string resolvedAddress, string forecastDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            var result = await _weatherAPIHandler.FetchDataAsync(resolvedAddress, user.GlobalMetric);
            if (result == null)
            {
                ViewBag.error = true;
                return View("~/Views/Home/Index.cshtml");
            }
            if (user.GlobalMetric)
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
            result.Metric = user.GlobalMetric;
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

        [HttpGet("Home/GetLocationOnMap")]
        [Authorize]
        public IActionResult GetLocationOnMap()
        {
            return PartialView("~/Views/Home/GetLocationOnMap.cshtml");
        }

        [HttpGet("Home/Shared")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForecastSharing(string city, Guid shareToken, Guid uid, bool metric, string? foreCastDate, bool? displayMultipleDays, int? DayAmount)
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
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                exists.ViewCount++;
            }
            await _context.SaveChangesAsync();
            var result = await _weatherAPIHandler.FetchDataAsync(city, metric);
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
            if (foreCastDate == null)
            {
                result.ForecastDate = DateTime.Now;
            }
            else
            {
                result.ForecastDate = DateTime.Parse(foreCastDate);
            }
            if (displayMultipleDays == true)
            {
                result.DisplayMultipleDays = true;
                result.DayAmount = DayAmount;
            }
            else
            {
                result.DisplayMultipleDays = false;
            }
            result.QrCodeBase64 = _qrCodeService.CreateSharedForecastQRCodeAsB64(new ShareDto() { City = city, Metric = metric, UID = uid, ShareToken = shareToken, Date = result.ForecastDate });
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
