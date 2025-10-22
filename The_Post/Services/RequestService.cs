using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using The_Post.Models;
using The_Post.Models.API;

namespace The_Post.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpClient = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<WeatherForecast> GetForecastAsync(string city)
        {
            // The public API endpoint for retrieving posts
            var url = $"http://weatherapi.dreammaker-it.se/forecast?city={city.ToLower()}&lang=eng";

            // Make an HTTP GET request and parse the data into a WeatherForecast object
            var weatherForecast = await _httpClient.GetFromJsonAsync<WeatherForecast>(url);

            // Makes the first character in each word upper-case. Only applied to the first word for summary.
            if (weatherForecast != null)
            {
                weatherForecast.Summary = char.ToUpper(weatherForecast.Summary[0]) + weatherForecast.Summary[1..];
                weatherForecast.City = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weatherForecast.City.ToLower());
            }

            // Return am empty forecast-object if null
            return weatherForecast ?? new WeatherForecast();
        }

        public async Task<List<WeatherForecast>> GetForecastsUserAsync()
        {
            var loggedInUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            // Logged-in user's "weather cities"
            var currentCities = loggedInUser.WeatherCities?.Split(',').Where(city => !string.IsNullOrEmpty(city)).ToList() ?? new List<string>();

            // Adds the user's local city if not already included
            if (!currentCities.Contains(loggedInUser.City))
            {
                currentCities.Insert(0, loggedInUser.City);  
            }

            List<WeatherForecast> foreCasts = new List<WeatherForecast>();

            // Adds forecast to the foreCasts-list, for each city in currentCities
            foreach (string city in currentCities)
            {
                try
                {
                    var foreCast = await GetForecastAsync(city);
                    if (foreCast != null)
                    {
                        foreCasts.Add(foreCast);
                    }
                }
                catch (Exception ex)
                {
                    // Needs to be changed
                    foreCasts.Add(new WeatherForecast() { City = city, Summary = "Error fetching data" });
                }
            }

            return foreCasts;
        }

        public async Task RemoveCity(string city)
        {
            var loggedInUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            var currentCities = loggedInUser.WeatherCities?.Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList() ?? new List<string>();

            // If the city is found, it is removed (case insensitive)
            if (currentCities.Contains(city, StringComparer.OrdinalIgnoreCase))
            {
                currentCities.Remove(currentCities.First(c => c.Equals(city, StringComparison.OrdinalIgnoreCase)));
            }

            // Updates the user's "weather cities"
            loggedInUser.WeatherCities = string.Join(",", currentCities);
            await _userManager.UpdateAsync(loggedInUser);
        }
    }
}
