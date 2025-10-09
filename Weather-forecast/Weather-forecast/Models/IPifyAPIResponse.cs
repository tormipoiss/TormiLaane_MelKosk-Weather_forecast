namespace Weather_forecast.Models
{
    public class As
    {
        public int asn { get; set; }
        public string name { get; set; }
        public string route { get; set; }
        public string domain { get; set; }
        public string type { get; set; }
    }

    public class Location
    {
        public string country { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string postalCode { get; set; }
        public string timezone { get; set; }
        public int geonameId { get; set; }
    }

    public class IPifyAPIResponse
    {
        public string ip { get; set; }
        public Location location { get; set; }
        public As @as { get; set; }
        public string isp { get; set; }
    }
}