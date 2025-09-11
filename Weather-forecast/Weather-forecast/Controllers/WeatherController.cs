using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Weather_forecast.Data;
using Weather_forecast.Models;
using Weather_forecast.Services;

namespace Weather_forecast.Controllers
{
    public class WeatherController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly WeatherAPIHandler _weatherAPIHandler;

        public WeatherController(DatabaseContext context, WeatherAPIHandler weatherAPIHandler)
        {
            _context = context;
            _weatherAPIHandler = weatherAPIHandler;
        }
        //[HttpGet]
        //public IActionResult SearchWeatherByCity(History model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("~/Views/Home/Index.cshtml", model);
        //    }
        //    if (model.City == null || model.City == "")
        //    {
        //        return View("~/Views/Home/Index.cshtml");
        //    }
        //    SaveCityToHistory(model.City);
        //    return View("~/Views/Home/Index.cshtml");
        //}

        [HttpGet]
        public async Task<IActionResult> SearchWeatherByCity(History model)
        {
            if (model.City == null || model.City == "")
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var result = await _weatherAPIHandler.FetchDataAsync(model.City);
            if (result == null) return BadRequest($"Failed to fetch weather data for {model.City}");
            SaveCityToHistory(model.City);
            return View(result);
        }

        [HttpPost]
        public IActionResult SaveCityToHistory(string city)
        {
            var history = new History();
            history.City = city;
            history.DateAndTimeOfSearch = DateTime.Now;
            _context.SearchHistory.Add(history);
            _context.SaveChanges();
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
