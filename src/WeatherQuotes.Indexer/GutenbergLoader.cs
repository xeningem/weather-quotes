using System.Text.RegularExpressions;

namespace WeatherQuotes.Indexer;

public record BookMetadata(string Era, string Genre, string Language);

public record LiteraryQuote(string Text, string Book, string Author, BookMetadata Metadata);

public static class GutenbergLoader
{
    private static readonly string[] WeatherKeywords =
    [
        // Precipitation
        "rain", "snow", "sleet", "hail", "drizzle", "shower", "downpour", "rainfall",
        "pelting", "snowfall", "snowflake", "snowdrift", "snowstorm", "flurry",
        "flood", "flooded", "flooding",
        // Wind
        "wind", "gale", "breeze", "squall", "gust", "tempest", "storm",
        "hurricane", "typhoon", "tornado", "whirlwind",
        // Temperature — cold
        "cold", "frost", "freeze", "freezing", "chill", "chilled",
        "ice", "icy", "icicle", "crisp", "brisk", "raw", "bitter", "wintry",
        "shiver", "shivering",
        // Temperature — heat
        "warm", "hot", "heat", "scorching", "sweltering", "muggy", "sultry", "stifling",
        "blazing", "baking", "parched", "drought",
        // Sky & visibility
        "fog", "mist", "haze", "cloud", "clouds", "overcast", "grey", "gray",
        "gloom", "gloomy", "murk", "darkness", "leaden", "dreary", "bleak",
        "twilight", "dusk", "dawn", "daylight", "sunlight",
        // Light & electricity
        "sun", "sunny", "sunshine", "moonlight", "starlight",
        "lightning", "thunder", "thunderstorm", "blizzard",
        // Moisture & dew
        "damp", "dampness", "wet", "wetness", "humid", "humidity",
        "dry", "dryness", "dew", "dewdrop",
        // Atmospheric
        "weather", "sky", "air", "atmosphere",
        // Seasonal
        "winter", "summer", "autumn", "fall", "spring", "season", "seasonal",
        // Compound descriptors
        "snow-covered", "snow-white", "wind-swept", "rain-soaked", "storm-tossed", "weather-beaten",
        // Mood / literary atmosphere
        "turbulent", "turbulence", "tempestuous", "placid", "tranquil", "boisterous", "blustering"
    ];

    private static readonly Dictionary<string, BookMetadata> BookMetadataLookup = new()
    {
        // Classic Victorian/19th century
        { "Emily Bronte - Wuthering Heights", new BookMetadata("Victorian", "novel", "en") },
        { "Charlotte Bronte - Jane Eyre", new BookMetadata("Victorian", "novel", "en") },
        { "Charles Dickens - Great Expectations", new BookMetadata("Victorian", "novel", "en") },
        { "Charles Dickens - David Copperfield", new BookMetadata("Victorian", "novel", "en") },
        { "Charles Dickens - Bleak House", new BookMetadata("Victorian", "novel", "en") },
        { "Thomas Hardy - Far from the Madding Crowd", new BookMetadata("Victorian", "novel", "en") },
        { "Thomas Hardy - The Return of the Native", new BookMetadata("Victorian", "novel", "en") },
        { "Thomas Hardy - Tess of the d'Urbervilles", new BookMetadata("Victorian", "novel", "en") },
        { "Thomas Hardy - Jude the Obscure", new BookMetadata("Late Victorian", "novel", "en") },
        { "Herman Melville - Moby-Dick", new BookMetadata("19th century", "novel", "en") },
        { "Fyodor Dostoyevsky - Crime and Punishment", new BookMetadata("19th century", "novel", "en") },
        { "Leo Tolstoy - Anna Karenina", new BookMetadata("19th century", "novel", "en") },
        { "Leo Tolstoy - War and Peace", new BookMetadata("19th century", "novel", "en") },
        { "Bram Stoker - Dracula", new BookMetadata("Victorian", "novel", "en") },
        { "Mary Shelley - Frankenstein", new BookMetadata("Early 19th century", "novel", "en") },
        { "Frances Hodgson Burnett - The Secret Garden", new BookMetadata("Edwardian", "novel", "en") },
        { "L.M. Montgomery - Anne of Green Gables", new BookMetadata("Edwardian", "novel", "en") },
        { "Arthur Conan Doyle - The Hound of the Baskervilles", new BookMetadata("Victorian", "novel", "en") },
        { "Jane Austen - Sense and Sensibility", new BookMetadata("Regency", "novel", "en") },
        { "Jane Austen - Pride and Prejudice", new BookMetadata("Regency", "novel", "en") },
        { "Mark Twain - Adventures of Huckleberry Finn", new BookMetadata("19th century", "novel", "en") },
        { "Joseph Conrad - Heart of Darkness", new BookMetadata("Late Victorian", "novella", "en") },
        { "Joseph Conrad - Lord Jim", new BookMetadata("Late Victorian", "novel", "en") },
        { "Joseph Conrad - The Nigger of the Narcissus", new BookMetadata("Late Victorian", "novella", "en") },
        { "Jack London - The Call of the Wild", new BookMetadata("Early 20th century", "novel", "en") },
        { "Jack London - White Fang", new BookMetadata("Early 20th century", "novel", "en") },
        { "Jack London - The Sea Wolf", new BookMetadata("Early 20th century", "novel", "en") },
        { "Jack London - The People of the Abyss", new BookMetadata("Early 20th century", "non-fiction", "en") },
        { "Anton Chekhov - The Cherry Orchard", new BookMetadata("Early 20th century", "play", "en") },
        { "Anton Chekhov - Ward Number Six", new BookMetadata("Late 19th century", "short story", "en") },
        // Batch 2
        { "Joseph Conrad - Typhoon", new BookMetadata("Edwardian", "novella", "en") },
        { "Joseph Conrad - The Secret Sharer", new BookMetadata("Edwardian", "short story", "en") },
        { "Thomas Hardy - The Mayor of Casterbridge", new BookMetadata("Victorian", "novel", "en") },
        { "Thomas Hardy - The Woodlanders", new BookMetadata("Victorian", "novel", "en") },
        { "Jack London - Burning Daylight", new BookMetadata("Early 20th century", "novel", "en") },
        { "Jack London - Martin Eden", new BookMetadata("Early 20th century", "novel", "en") },
        { "Anton Chekhov - Uncle Vanya", new BookMetadata("Late 19th century", "play", "en") },
        { "Anton Chekhov - Three Sisters", new BookMetadata("Early 20th century", "play", "en") },
        { "George Eliot - Middlemarch", new BookMetadata("Victorian", "novel", "en") },
        { "George Eliot - The Mill on the Floss", new BookMetadata("Victorian", "novel", "en") },
        { "D.H. Lawrence - Sons and Lovers", new BookMetadata("Early 20th century", "novel", "en") },
        { "D.H. Lawrence - Women in Love", new BookMetadata("Early 20th century", "novel", "en") },
    };

    public static List<LiteraryQuote> LoadBooks(string booksDirectory)
    {
        var quotes = new List<LiteraryQuote>();

        if (!Directory.Exists(booksDirectory))
        {
            Console.WriteLine($"Directory '{booksDirectory}' not found.");
            return quotes;
        }

        foreach (var file in Directory.GetFiles(booksDirectory, "*.txt").OrderBy(f => f))
        {
            var (book, author) = ParseMetadata(file);
            var metadata = GetMetadata(file);
            var paragraphs = ExtractWeatherParagraphs(file);
            quotes.AddRange(paragraphs.Select(p => new LiteraryQuote(p, book, author, metadata)));
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

    private static BookMetadata GetMetadata(string filePath)
    {
        var filename = Path.GetFileNameWithoutExtension(filePath);
        return BookMetadataLookup.TryGetValue(filename, out var meta)
            ? meta
            : new BookMetadata("Unknown", "novel", "en");
    }

    private static IEnumerable<string> ExtractWeatherParagraphs(string filePath)
    {
        var text = File.ReadAllText(filePath);

        var gutenbergStart = Regex.Match(text, @"\*\*\* START OF .*? \*\*\*", RegexOptions.IgnoreCase);
        if (gutenbergStart.Success)
            text = text[(gutenbergStart.Index + gutenbergStart.Length)..];

        // Re-match END on the already-trimmed text so the index is correct.
        var gutenbergEnd = Regex.Match(text, @"\*\*\* END OF .*? \*\*\*", RegexOptions.IgnoreCase);
        if (gutenbergEnd.Success)
            text = text[..gutenbergEnd.Index];

        var paragraphs = Regex.Split(text, @"\n\s*\n")
            .Select(p => Regex.Replace(p.Trim(), @"\s+", " "))
            .Where(p => p.Length >= 120 && p.Length <= 1200)
            .Where(ContainsWeatherKeyword);

        return paragraphs;
    }

    private static bool ContainsWeatherKeyword(string text)
    {
        var lower = text.ToLowerInvariant();
        return WeatherKeywords.Any(kw => lower.Contains(kw));
    }
}
