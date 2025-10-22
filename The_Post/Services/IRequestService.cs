using The_Post.Models.API;

namespace The_Post.Services
{
    public interface IRequestService
    {
        public Task<WeatherForecast> GetForecastAsync(string city);
        public Task<List<WeatherForecast>> GetForecastsUserAsync();
        public Task RemoveCity(string city);
    }
}
