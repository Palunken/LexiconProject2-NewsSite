using Newtonsoft.Json;

namespace The_Post.Models.API
{
    public class WeatherForecast
    {
        public string Summary { get; set; }
        public string City { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public DateTime Date { get; set; }
        public int UnixTime { get; set; }
        public Icon Icon { get; set; }
    }

    public class Icon
    {
        public string Url { get; set; }
        public string Code { get; set; }
    }
}

