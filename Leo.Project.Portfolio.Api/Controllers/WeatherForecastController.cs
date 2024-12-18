using Microsoft.AspNetCore.Mvc;
 

namespace Leo.Project.Portfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This will route to api/weatherforecast by default
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        // GET api/getweather
        [HttpGet("getweather")]  // Custom route
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // POST api/AddWeather (This will add a new weather forecast)
        [HttpPost("AddWeather")]  // Custom route for adding weather
        public IActionResult AddWeather([FromBody] WeatherForecast newWeather)
        {
            // For demonstration purposes, let's return the received weather data
            if (newWeather == null)
            {
                return BadRequest("Weather data is required.");
            }

            // You would typically save this data to a database here

            // Return a response indicating success and the added weather
            return CreatedAtAction(nameof(Get), new { id = newWeather.Date }, newWeather);
        }

    }
}
