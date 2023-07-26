using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController] // Attributes
// NOTES: This is an API controller attribute, used to serve HTTP API responses
[Route("[controller]")] // Each controller has a route, so the App knows where to redirect the HTTP Request to
/* NOTES: The "[controller]" is a placeholder for the actual name of the controller
- When we access this route, we type: localhost:5000/weatherforecast
- Which is the prefix of the class below: WeatherForecastCONTROLLER
*/
public class WeatherForecastController : ControllerBase
{
    // NOTES: Each API controller derives from a base class "ControllerBase"
    // Notes: When hovering over "ControllerBase", I can see that it comes from "Microsoft.AspNetCore.Mvc".
    // - When I don't import ("using") this dependency, I becomes unavailable.
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    // A private array of strings^

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }
    /* NOTES: This is dependency injection^
    - We're making something available to our controller: "_logger"
    
    */

    // localhost:5000/weatherforecast/getweatherforecast => [gets data declared in method below]
    [HttpGet(Name = "GetWeatherForecast")] // An Endpoint specifies what crud operation () is happening
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
}
