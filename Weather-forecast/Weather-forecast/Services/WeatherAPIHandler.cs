using System.Diagnostics;
using System.Text.Json;
using Weather_forecast.Models;
using Weather_forecast.ViewModels;

namespace Weather_forecast.Services
{
    public class WeatherAPIHandler
    {
        private readonly HttpClient _httpClient;
        //https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/Tallinn%2C%2037%2C%20EE?unitGroup=metric&key=WMB6TMRW43KZPHAS6L8UHCN2S&contentType=json
        private static readonly string _apiKey = "WMB6TMRW43KZPHAS6L8UHCN2S"; // dont hardcode secrets
        private static readonly string _baseUrl = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{0}"+$"?unitGroup=metric&key={_apiKey}&contentType=json";
        public WeatherAPIHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CityAndApi?> FetchDataAsync(string city)
        {
            try
            {
                var res = await _httpClient.GetAsync(string.Format(_baseUrl,city));
                res.EnsureSuccessStatusCode();
                string content = await res.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<WeatherforcecastAPIResponseModel>(content);
                CityAndApi fixedvalues = new CityAndApi();
                fixedvalues.Weather = values;
                string uriLatitude = values.latitude.ToString().Replace(",", ".");
                string uriLongitude = values.longitude.ToString().Replace(",", ".");
                fixedvalues.EmbedUrl = $"https://maps.google.com/maps?q={uriLatitude}+{uriLongitude}&t=k&z=15&ie=UTF8&iwloc=&output=embed";
                return fixedvalues;
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"Failed to make a web req to weather forecast API: {httpEx.Message}.\nStatus code: {httpEx.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to deserialize json. {ex.Message}");
            }
            return null;
        }
    }
}
