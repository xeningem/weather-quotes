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
                var quotes = await quoteSearchService.SearchAsync(weather.ToNaturalLanguage(), weather.TemperatureCelsius, weather.Condition);
                return Results.Ok(new { weather, quotes });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetQuote");
    }
}
