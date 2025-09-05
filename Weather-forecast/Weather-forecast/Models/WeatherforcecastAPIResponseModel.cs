namespace Weather_forecast.Models
{
    public class CurrentConditions
    {
        public string datetime { get; set; }
        public int datetimeEpoch { get; set; }
        public double temp { get; set; }
        public double feelslike { get; set; }
        public double humidity { get; set; }
        public double dew { get; set; }
        public double precip { get; set; }
        public double precipprob { get; set; }
        public double snow { get; set; }
        public double snowdepth { get; set; }
        public double windgust { get; set; }
        public double windspeed { get; set; }
        public double winddir { get; set; }
        public double pressure { get; set; }
        public double visibility { get; set; }
        public double cloudcover { get; set; }
        public double solarradiation { get; set; }
        public double solarenergy { get; set; }
        public double uvindex { get; set; }
        public string conditions { get; set; }
        public string icon { get; set; }
        public IList<string> stations { get; set; }
        public string source { get; set; }
        public string sunrise { get; set; }
        public int sunriseEpoch { get; set; }
        public string sunset { get; set; }
        public int sunsetEpoch { get; set; }
        public double moonphase { get; set; }

    }
    public class WeatherforcecastAPIResponseModel
    {
        public int queryCost { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string resolvedAddress { get; set; }
        public string address { get; set; }
        public string timezone { get; set; }
        public double tzoffset { get; set; }
        public string description { get; set; }
        public object days { get; set; }
        public object alerts { get; set; }
        public object stations { get; set; }
        public CurrentConditions currentConditions { get; set; }
    }
}
