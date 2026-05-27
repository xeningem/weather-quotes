using Qdrant.Client;
using WeatherQuotes.Api.Models;

namespace WeatherQuotes.Api.Services;

public class QuoteSearchService(QdrantClient qdrant, EmbeddingService embeddingService)
{
    public const string CollectionName = "literary_quotes";

    public async Task<IReadOnlyList<QuoteResult>> SearchAsync(string weatherDescription, double temperatureCelsius, string condition, int limit = 3)
    {
        var vector = await embeddingService.GetEmbeddingAsync(weatherDescription);

        var results = await qdrant.SearchAsync(
            CollectionName,
            vector,
            limit: 40,
            payloadSelector: true
        );

        var all = results.Select(r => new QuoteResult(
            Text: r.Payload["text"].StringValue,
            Book: r.Payload["book"].StringValue,
            Author: r.Payload["author"].StringValue,
            Score: r.Score
        )).ToList();

        var filtered = all
            .Where(q => IsTemperatureCompatible(q.Text, temperatureCelsius))
            .Where(q => IsConditionCompatible(q.Text, condition))
            .ToList();

        var pool = (filtered.Count >= limit ? filtered : all).Take(15).ToList();

        return pool.OrderBy(_ => Random.Shared.Next()).Take(limit).ToList();
    }

    private static bool IsTemperatureCompatible(string text, double temp)
    {
        var lower = text.ToLowerInvariant();
        if (temp >= 18)
            return !ContainsAny(lower, "bitterly cold", "freezing cold", "bitter frost",
                "ice cold", "numbing cold", "frozen stiff", "teeth chattering",
                "cold weather", "cold air", "cold wind", "cold morning", "cold night",
                "cold day", "cold evening", "cold and", "cold, bright", "frost");
        if (temp <= 5)
            return !ContainsAny(lower, "sweltering", "scorching", "blazing heat",
                "oppressive heat", "stifling heat", "burning hot", "stifling",
                "sultry", "torrid");
        return true;
    }

    private static bool IsConditionCompatible(string text, string condition)
    {
        var lower = text.ToLowerInvariant();
        return condition switch
        {
            "Thunderstorm" or "Rain" or "Drizzle" =>
                !ContainsAny(lower, "blazing sun", "scorching sun", "bright sunshine",
                    "cloudless sky", "clear blue sky", "brilliant sunshine",
                    "hot sunshine", "the sun beat", "sun beat down", "not a cloud"),
            "Clear" =>
                !ContainsAny(lower, "thunder", "thunderstorm", "lightning",
                    "downpour", "torrential", "blizzard", "snowstorm"),
            "Snow" =>
                !ContainsAny(lower, "sweltering", "scorching", "blazing heat",
                    "summer heat", "hot sunshine"),
            _ => true
        };
    }

    private static bool ContainsAny(string s, params string[] terms) =>
        terms.Any(s.Contains);
}
