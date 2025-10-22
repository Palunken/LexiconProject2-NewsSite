using The_Post.Models.API;

namespace The_Post.Models.VM
{
    public class WeatherVM
    {
        public List<WeatherForecast> Forecasts = new List<WeatherForecast>();
        public List<Article> Articles = new List<Article>();
    }
}
