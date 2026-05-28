using Microsoft.Playwright;

namespace WeatherQuotes.Tests.Browser;

/// <summary>
/// Browser E2E tests — require the API running at http://localhost:5293.
/// Run with: dotnet test --filter "Category=Browser"
/// First-time setup: pwsh tests/WeatherQuotes.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
/// </summary>
[Collection("Browser")]
[Trait("Category", "Browser")]
public class UiSmokeTests : IAsyncLifetime
{
    private const string BaseUrl = "http://localhost:5293";

    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    private async Task<(IPage page, List<string> errors)> NewPageWithErrorCapture()
    {
        var page = await _browser.NewPageAsync();
        var errors = new List<string>();
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error") errors.Add(msg.Text);
        };
        page.PageError += (_, msg) => errors.Add(msg);
        return (page, errors);
    }

    private async Task<IPage> SearchAndWaitForCard(string city)
    {
        var (page, _) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", city);
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });
        return page;
    }

    [Fact]
    public async Task PageLoad_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Search_London_ShowsQuoteCard_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);

        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        Assert.Empty(errors);
        var cardText = await page.TextContentAsync(".quote-text");
        Assert.False(string.IsNullOrWhiteSpace(cardText));
    }

    [Fact]
    public async Task ScoreToggle_ShowsPercentLabel_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        await page.ClickAsync("button:has-text('Score')");

        var meta = await page.TextContentAsync(".quote-meta");
        Assert.Contains("% match", meta);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ProseToggle_ShowsProseBlock_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        await page.ClickAsync("button:has-text('Prose')");

        var proseBlock = page.Locator(".prose-block");
        await Assertions.Expect(proseBlock).ToBeVisibleAsync();
        var proseText = await proseBlock.TextContentAsync();
        Assert.False(string.IsNullOrWhiteSpace(proseText));
        Assert.Empty(errors);
    }

    [Fact]
    public async Task HighlightsToggle_WrapsWordsInMark_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        await page.ClickAsync("button:has-text('Highlights')");

        var markCount = await page.Locator(".quote-text mark").CountAsync();
        Assert.True(markCount > 0, $"Expected at least one highlighted word, found {markCount}");
        Assert.Empty(errors);
    }

    [Fact]
    public async Task AllTogglesOn_NoConsoleErrors()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        await page.ClickAsync("button:has-text('Score')");
        await page.ClickAsync("button:has-text('Prose')");
        await page.ClickAsync("button:has-text('Highlights')");

        await page.WaitForTimeoutAsync(200);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task NewSearch_ResetsTogglesToOff()
    {
        var (page, errors) = await NewPageWithErrorCapture();
        await page.GotoAsync(BaseUrl);
        await page.FillAsync("input[type=text]", "London");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        await page.ClickAsync("button:has-text('Score')");
        await page.ClickAsync("button:has-text('Prose')");
        await page.ClickAsync("button:has-text('Highlights')");

        var scoreActive = await page.Locator("button:has-text('Score').active").CountAsync();
        Assert.Equal(1, scoreActive);

        await page.FillAsync("input[type=text]", "Paris");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 30_000 });

        var scoreActiveAfter = await page.Locator("button:has-text('Score').active").CountAsync();
        Assert.Equal(0, scoreActiveAfter);

        var proseVisible = await page.Locator(".prose-block").IsVisibleAsync();
        Assert.False(proseVisible);

        var markCount = await page.Locator(".quote-text mark").CountAsync();
        Assert.Equal(0, markCount);

        Assert.Empty(errors);
    }
}
