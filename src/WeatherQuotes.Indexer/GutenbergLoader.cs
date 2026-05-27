using System.Text.RegularExpressions;

namespace WeatherQuotes.Indexer;

public record LiteraryQuote(string Text, string Book, string Author);

public static class GutenbergLoader
{
    private static readonly string[] WeatherKeywords =
    [
        "rain", "snow", "wind", "storm", "fog", "mist", "cloud", "sun", "frost",
        "thunder", "lightning", "drizzle", "hail", "sleet", "blizzard", "gale",
        "weather", "sky", "cold", "warm", "heat", "chill", "damp", "wet", "dry",
        "humid", "freezing", "scorching", "overcast", "twilight", "dawn", "dusk",
        "moonlight", "starlight", "breeze", "shower", "tempest", "gloom", "grey"
    ];

    public static List<LiteraryQuote> LoadBooks(string booksDirectory)
    {
        var quotes = new List<LiteraryQuote>();

        if (!Directory.Exists(booksDirectory))
        {
            Console.WriteLine($"Directory '{booksDirectory}' not found.");
            return quotes;
        }

        foreach (var file in Directory.GetFiles(booksDirectory, "*.txt"))
        {
            var (book, author) = ParseMetadata(file);
            var paragraphs = ExtractWeatherParagraphs(file);
            quotes.AddRange(paragraphs.Select(p => new LiteraryQuote(p, book, author)));
        }

        return quotes;
    }

    private static (string book, string author) ParseMetadata(string filePath)
    {
        var filename = Path.GetFileNameWithoutExtension(filePath);
        var parts = filename.Split(" - ", 2);
        return parts.Length == 2
            ? (parts[1].Trim(), parts[0].Trim())
            : (filename, "Unknown");
    }

    private static IEnumerable<string> ExtractWeatherParagraphs(string filePath)
    {
        var text = File.ReadAllText(filePath);

        var gutenbergStart = Regex.Match(text, @"\*\*\* START OF .*? \*\*\*", RegexOptions.IgnoreCase);
        var gutenbergEnd = Regex.Match(text, @"\*\*\* END OF .*? \*\*\*", RegexOptions.IgnoreCase);

        if (gutenbergStart.Success)
            text = text[(gutenbergStart.Index + gutenbergStart.Length)..];
        if (gutenbergEnd.Success)
            text = text[..gutenbergEnd.Index];

        var paragraphs = Regex.Split(text, @"\n\s*\n")
            .Select(p => Regex.Replace(p.Trim(), @"\s+", " "))
            .Where(p => p.Length >= 60 && p.Length <= 800)
            .Where(ContainsWeatherKeyword);

        return paragraphs;
    }

    private static bool ContainsWeatherKeyword(string text)
    {
        var lower = text.ToLowerInvariant();
        return WeatherKeywords.Any(kw => lower.Contains(kw));
    }
}
