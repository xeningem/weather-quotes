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
            IQuoteSearchService quoteSearchService,
            int offset = 0) =>
        {
            if (offset < 0 || offset > 3)
                return Results.BadRequest(new { error = "offset must be between 0 and 3" });

            try
            {
                var weather = offset == 0
                    ? await weatherService.GetCurrentWeatherAsync(location)
                    : await weatherService.GetForecastWeatherAsync(location, offset);
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
