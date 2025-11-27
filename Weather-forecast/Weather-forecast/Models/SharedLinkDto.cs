namespace Weather_forecast.Models
{
    public class SharedLinkDto
    {
        public bool MultipleDays { get; set; }
        public string CityName { get; set; }
        public string ShareToken { get; set; }
        public string UserId { get; set; }
        public bool IsMetric { get; set; }
        public DateTime? Date { get; set; }
        public int? DayAmount { get; set; }
    }
}
