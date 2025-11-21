using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using QRCoder;
using System.Drawing;
using System.Text.Json;
using Weather_forecast.Models;

namespace Weather_forecast.Services
{
    public class LocationByIPService
    {
        public LocationByIPService()
        {
        }
        public async Task<string> LocationByIP()
        {
            HttpClient _httpClient = new HttpClient();
            var resIP = await _httpClient.GetAsync("https://api.ipify.org");
            resIP.EnsureSuccessStatusCode();
            string ipAddress = await resIP.Content.ReadAsStringAsync();

            var resIPLocation = await _httpClient.GetAsync($"https://geo.ipify.org/api/v2/country,city?apiKey=at_0tVUddlB1XlCDbJBhAskOnQwiIcsr&ipAddress={ipAddress}");
            resIPLocation.EnsureSuccessStatusCode();
            string content = await resIPLocation.Content.ReadAsStringAsync();
            var IpInfo = JsonSerializer.Deserialize<IPifyAPIResponse>(content);

            return IpInfo.location.city;
        }
    }
}
