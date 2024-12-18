using System.Net.Http.Json;
using static Leo.Project.Portfolio.Web.Pages.Weather;

namespace Leo.Project.Portfolio.Web.Services;

public class PortfolioApiService
{
    private readonly HttpClient _httpClient;

    public PortfolioApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
 
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        // Adjust the API endpoint if needed, here it's assumed that you have /api/weatherforecast in your API
        var response = await _httpClient.GetAsync("api/WeatherForecast/getweather");
        response.EnsureSuccessStatusCode();

        var weatherForecasts = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
        return weatherForecasts;
    } 
}