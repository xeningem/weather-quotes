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

        app.MapGet("/api/quote/prose", async (
            string text,
            IQuoteSearchService quoteSearchService) =>
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim().Length < 5)
                return Results.BadRequest(new { error = "text is too short" });
            if (text.Length > 1000)
                return Results.BadRequest(new { error = "text is too long (max 1000 chars)" });

            var prose = text.Trim();
            // Temperature 12 and empty condition are neutral — no keyword filtering applied
            var quotes = await quoteSearchService.SearchAsync(prose, 12.0, "");
            return Results.Ok(new { weatherProse = prose, quotes });
        })
        .WithName("GetQuoteFromProse");
    }
}
