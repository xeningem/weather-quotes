using WeatherQuotes.Api.Models;
using WeatherQuotes.Api.Services;

namespace WeatherQuotes.Api.Endpoints;

public static class QuoteEndpoints
{
    public static void MapQuoteEndpoints(this WebApplication app)
    {
        app.MapGet("/api/quote", async (
            string location,
            IWeatherService weatherService,
            IQuoteSearchService quoteSearchService) =>
        {
            try
            {
                var weather = await weatherService.GetCurrentWeatherAsync(location);
                var weatherProse = weather.ToNaturalLanguage();
                var quotes = await quoteSearchService.SearchAsync(weatherProse, weather.TemperatureCelsius, weather.Condition);
                return Results.Ok(new { weather, weatherProse, quotes });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetQuote");
    }
}
