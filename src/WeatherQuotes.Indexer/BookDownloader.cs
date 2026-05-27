namespace WeatherQuotes.Indexer;

public static class BookDownloader
{
    private static readonly (int Id, string Author, string Title)[] Catalog =
    [
        (768,   "Emily Bronte",            "Wuthering Heights"),
        (1260,  "Charlotte Bronte",        "Jane Eyre"),
        (1400,  "Charles Dickens",         "Great Expectations"),
        (766,   "Charles Dickens",         "David Copperfield"),
        (1023,  "Charles Dickens",         "Bleak House"),
        (107,   "Thomas Hardy",            "Far from the Madding Crowd"),
        (122,   "Thomas Hardy",            "The Return of the Native"),
        (110,   "Thomas Hardy",            "Tess of the d'Urbervilles"),
        (1591,  "Thomas Hardy",            "Jude the Obscure"),
        (2701,  "Herman Melville",         "Moby-Dick"),
        (2554,  "Fyodor Dostoyevsky",      "Crime and Punishment"),
        (1399,  "Leo Tolstoy",             "Anna Karenina"),
        (2600,  "Leo Tolstoy",             "War and Peace"),
        (345,   "Bram Stoker",             "Dracula"),
        (84,    "Mary Shelley",            "Frankenstein"),
        (113,   "Frances Hodgson Burnett", "The Secret Garden"),
        (45,    "L.M. Montgomery",         "Anne of Green Gables"),
        (2852,  "Arthur Conan Doyle",      "The Hound of the Baskervilles"),
        (161,   "Jane Austen",             "Sense and Sensibility"),
        (1342,  "Jane Austen",             "Pride and Prejudice"),
        (76,    "Mark Twain",              "Adventures of Huckleberry Finn"),
        (1357,  "Joseph Conrad",           "Heart of Darkness"),
        (42,    "Joseph Conrad",           "Lord Jim"),
        (1919,  "Joseph Conrad",           "The Nigger of the Narcissus"),
        (2814,  "Jack London",             "The Call of the Wild"),
        (2262,  "Jack London",             "White Fang"),
        (3407,  "Jack London",             "The Sea Wolf"),
        (4667,  "Jack London",             "The People of the Abyss"),
        (1798,  "Anton Chekhov",           "The Cherry Orchard"),
        (2738,  "Anton Chekhov",           "Ward Number Six"),
        // Batch 2 — weather-rich additions
        (1723,  "Joseph Conrad",           "Typhoon"),
        (220,   "Joseph Conrad",           "The Secret Sharer"),
        (143,   "Thomas Hardy",            "The Mayor of Casterbridge"),
        (1582,  "Thomas Hardy",            "The Woodlanders"),
        (3004,  "Jack London",             "Burning Daylight"),
        (1056,  "Jack London",             "Martin Eden"),
        (1756,  "Anton Chekhov",           "Uncle Vanya"),
        (7986,  "Anton Chekhov",           "Three Sisters"),
        (13415, "Anton Chekhov",           "The Lady with the Dog and Other Stories"),
        (13416, "Anton Chekhov",           "The Darling and Other Stories"),
        (145,   "George Eliot",            "Middlemarch"),
        (6688,  "George Eliot",            "The Mill on the Floss"),
        (217,   "D.H. Lawrence",           "Sons and Lovers"),
        (4240,  "D.H. Lawrence",           "Women in Love"),
    ];


    public static async Task DownloadMissingAsync(string booksDirectory)
    {
        Directory.CreateDirectory(booksDirectory);

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "WeatherQuotes/1.0 (educational project)");

        foreach (var (id, author, title) in Catalog)
        {
            var fileName = $"{author} - {title}.txt";
            var filePath = Path.Combine(booksDirectory, fileName);

            if (File.Exists(filePath))
            {
                Console.WriteLine($"  [skip] {fileName}");
                continue;
            }

            Console.Write($"  [download] {fileName} ... ");
            try
            {
                var text = await TryDownloadAsync(http, id);
                if (text is null)
                {
                    Console.WriteLine("not found");
                    continue;
                }
                await File.WriteAllTextAsync(filePath, text);
                Console.WriteLine("ok");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }

            await Task.Delay(500);
        }
    }

    private static async Task<string?> TryDownloadAsync(HttpClient http, int id)
    {
        var candidates = new[]
        {
            $"https://www.gutenberg.org/cache/epub/{id}/pg{id}.txt",
            $"https://www.gutenberg.org/files/{id}/{id}-0.txt",
            $"https://www.gutenberg.org/files/{id}/{id}.txt",
        };

        foreach (var url in candidates)
        {
            var response = await http.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
        }

        return null;
    }
}
