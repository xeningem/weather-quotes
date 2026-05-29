using Microsoft.Playwright;

namespace WeatherQuotes.Tests.Browser;

/// <summary>
/// Takes screenshots for the README. Run once with:
///   dotnet test --filter "Category=Screenshot"
/// Requires the API running at http://localhost:5293.
/// </summary>
[Collection("Browser")]
[Trait("Category", "Screenshot")]
public class ScreenshotTests : IAsyncLifetime
{
    private const string BaseUrl = "http://localhost:5293";
    private const string OutDir = @"..\..\..\..\..\docs\screenshots";

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

    private async Task<IPage> NewPage(int width = 720)
    {
        var ctx = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = width, Height = 900 }
        });
        var page = await ctx.NewPageAsync();
        page.SetDefaultTimeout(60_000);
        return page;
    }

    [Fact]
    public async Task Screenshot_StartScreen()
    {
        var page = await NewPage();
        await page.GotoAsync(BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.ScreenshotAsync(new() { Path = Path.Combine(OutDir, "start.png"), FullPage = true });
    }

    [Fact]
    public async Task Screenshot_MainResult_London()
    {
        var page = await NewPage();
        await page.GotoAsync(BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.FillAsync(".search-wrap input[type=text]", "London");
        await page.ClickAsync(".search-wrap .btn-find");
        await page.WaitForSelectorAsync(".quote-card");
        await page.EvaluateAsync("Alpine.$data(document.querySelector('[x-data]')).suggestions = []");
        await page.WaitForTimeoutAsync(400);
        await page.ScreenshotAsync(new() { Path = Path.Combine(OutDir, "london-quote.png"), FullPage = true });
    }

    [Fact]
    public async Task Screenshot_DebugToggles_London()
    {
        var page = await NewPage();
        await page.GotoAsync(BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.FillAsync(".search-wrap input[type=text]", "London");
        await page.ClickAsync(".search-wrap .btn-find");
        await page.WaitForSelectorAsync(".quote-card");
        await page.EvaluateAsync("Alpine.$data(document.querySelector('[x-data]')).suggestions = []");
        await page.ClickAsync("button:has-text('Prose')");
        await page.ClickAsync("button:has-text('Score')");
        await page.ClickAsync("button:has-text('Highlights')");
        await page.WaitForTimeoutAsync(400);
        await page.ScreenshotAsync(new() { Path = Path.Combine(OutDir, "debug-toggles.png"), FullPage = true });
    }

    [Fact]
    public async Task Screenshot_CustomWeather_Russian()
    {
        var page = await NewPage();
        await page.GotoAsync(BaseUrl);
        await page.ClickAsync("button:has-text('Custom weather')");
        await page.FillAsync("textarea", "Густой зимний туман, мороз, тишина. Всё скрыто в серой мгле, деревья стоят как призраки.");
        await page.ClickAsync(".btn-find");
        await page.WaitForSelectorAsync(".quote-card", new() { Timeout = 40_000 });
        await page.WaitForTimeoutAsync(300);
        await page.ScreenshotAsync(new() { Path = Path.Combine(OutDir, "custom-weather-ru.png"), FullPage = true });
    }
}
