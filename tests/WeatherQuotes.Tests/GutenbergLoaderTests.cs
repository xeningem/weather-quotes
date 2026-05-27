using WeatherQuotes.Indexer;

namespace WeatherQuotes.Tests;

public class GutenbergLoaderTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public GutenbergLoaderTests() => Directory.CreateDirectory(_dir);
    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private void WriteBook(string filename, string content) =>
        File.WriteAllText(Path.Combine(_dir, filename), content);

    // A paragraph that is exactly long enough and contains a weather keyword.
    private const string GoodParagraph =
        "The rain fell in heavy sheets upon the ancient stone walls of the old house, " +
        "drumming against the windows and filling the dark night air with cold dampness " +
        "that seeped into every corner of the room.";

    [Fact]
    public void LoadBooks_IncludesGoodWeatherParagraph()
    {
        WriteBook("Author - Title.txt", GoodParagraph);
        var quotes = GutenbergLoader.LoadBooks(_dir);
        Assert.Single(quotes);
    }

    [Fact]
    public void LoadBooks_ParsesAuthorAndBook()
    {
        WriteBook("Jane Austen - Pride and Prejudice.txt", GoodParagraph);
        var quote = Assert.Single(GutenbergLoader.LoadBooks(_dir));
        Assert.Equal("Jane Austen", quote.Author);
        Assert.Equal("Pride and Prejudice", quote.Book);
    }

    [Fact]
    public void LoadBooks_LooksUpKnownBookMetadata()
    {
        WriteBook("Jane Austen - Pride and Prejudice.txt", GoodParagraph);
        var quote = Assert.Single(GutenbergLoader.LoadBooks(_dir));
        Assert.Equal("Regency", quote.Metadata.Era);
        Assert.Equal("novel", quote.Metadata.Genre);
        Assert.Equal("en", quote.Metadata.Language);
    }

    [Fact]
    public void LoadBooks_UnknownBookGetsDefaultMetadata()
    {
        WriteBook("Unknown Author - Some Book.txt", GoodParagraph);
        var quote = Assert.Single(GutenbergLoader.LoadBooks(_dir));
        Assert.Equal("Unknown", quote.Metadata.Era);
    }

    [Fact]
    public void LoadBooks_ExcludesShortParagraphs()
    {
        // 119 chars with keyword — just under the 120-char minimum
        var shortPara = "The rain fell. " + new string('x', 104);
        Assert.True(shortPara.Length < 120);
        WriteBook("Author - Title.txt", shortPara);
        Assert.Empty(GutenbergLoader.LoadBooks(_dir));
    }

    [Fact]
    public void LoadBooks_ExcludesLongParagraphs()
    {
        // 1201 chars with keyword — just over the 1200-char maximum
        var longPara = "rain " + new string('x', 1196);
        Assert.True(longPara.Length > 1200);
        WriteBook("Author - Title.txt", longPara);
        Assert.Empty(GutenbergLoader.LoadBooks(_dir));
    }

    [Fact]
    public void LoadBooks_ExcludesParagraphsWithNoKeywords()
    {
        var noKeyword = new string('a', 200);
        WriteBook("Author - Title.txt", noKeyword);
        Assert.Empty(GutenbergLoader.LoadBooks(_dir));
    }

    [Fact]
    public void LoadBooks_StripsGutenbergHeader()
    {
        var preamble = new string('x', 200) + " rain " + new string('x', 200);
        var content =
            preamble + "\n\n" +
            "*** START OF THE PROJECT GUTENBERG EBOOK FOO ***\n\n" +
            GoodParagraph + "\n\n" +
            "*** END OF THE PROJECT GUTENBERG EBOOK FOO ***\n\n" +
            preamble;

        WriteBook("Author - Title.txt", content);
        var quotes = GutenbergLoader.LoadBooks(_dir);
        Assert.Single(quotes);
        Assert.Equal(GoodParagraph, quotes[0].Text);
    }

    [Fact]
    public void LoadBooks_ReturnsEmptyForMissingDirectory()
    {
        Assert.Empty(GutenbergLoader.LoadBooks("/nonexistent/path/xyz123"));
    }

    [Fact]
    public void LoadBooks_SortsFilesForDeterministicOrder()
    {
        // Two books — result order must follow alphabetical file name order
        var paraA = "The cold frost settled upon the ancient stone walls, filling the air with bitter chill and ice.";
        var paraB = GoodParagraph;
        WriteBook("B Author - Book B.txt", paraA + " " + paraA);
        WriteBook("A Author - Book A.txt", paraB);

        var quotes = GutenbergLoader.LoadBooks(_dir);
        Assert.True(quotes.Count >= 2);
        Assert.Equal("Book A", quotes[0].Book);
    }
}
