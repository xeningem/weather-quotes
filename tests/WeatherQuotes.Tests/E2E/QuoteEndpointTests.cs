using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WeatherQuotes.Api.Models;
using WeatherQuotes.Api.Services;

namespace WeatherQuotes.Tests.E2E;

public class QuoteEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public QuoteEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient BuildClient(IWeatherService? weather = null, IQuoteSearchService? search = null)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(weather ?? new StubWeatherService());
                services.AddSingleton(search ?? new StubQuoteSearchService());
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetQuote_ReturnsOk_WithWeatherAndQuotes()
    {
        var client = BuildClient();
        var response = await client.GetAsync("/api/quote?location=London");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<QuoteResponse>();
        Assert.NotNull(body);
        Assert.Equal("London, UK", body!.Weather.Location);
        Assert.Single(body.Quotes);
        Assert.Equal("It was a dark and stormy night.", body.Quotes[0].Text);
    }

    [Fact]
    public async Task GetQuote_MissingLocation_ReturnsBadRequest()
    {
        var client = BuildClient();
        var response = await client.GetAsync("/api/quote");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetQuote_WhenWeatherServiceThrows_ReturnsBadRequest()
    {
        var client = BuildClient(weather: new ThrowingWeatherService());
        var response = await client.GetAsync("/api/quote?location=Nowhere");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(body?.Error);
        Assert.Contains("Nowhere", body!.Error);
    }

    [Fact]
    public async Task GetQuote_ResponseIncludesAllExpectedFields()
    {
        var client = BuildClient();
        var body = await client
            .GetFromJsonAsync<QuoteResponse>("/api/quote?location=London");

        Assert.NotNull(body);
        Assert.NotNull(body!.Weather.Condition);
        Assert.NotNull(body.Weather.Description);
        Assert.True(body.Weather.TemperatureCelsius != 0 || body.Weather.HumidityPercent >= 0);
    }

    [Fact]
    public async Task GetQuote_QuotesIncludeMetadataFields()
    {
        var client = BuildClient();
        var body = await client
            .GetFromJsonAsync<QuoteResponse>("/api/quote?location=London");

        Assert.NotNull(body);
        var quote = Assert.Single(body!.Quotes);
        Assert.Equal("Victorian", quote.Era);
        Assert.Equal("novel", quote.Genre);
        Assert.Equal("en", quote.Language);
    }

    [Fact]
    public async Task GetQuote_ResponseIncludesWeatherProse()
    {
        var client = BuildClient();
        var body = await client
            .GetFromJsonAsync<QuoteResponse>("/api/quote?location=London");

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.WeatherProse));
    }

    [Fact]
    public async Task GetQuote_WithOffset1_ReturnsOkWithDate()
    {
        var client = BuildClient();
        var body = await client.GetFromJsonAsync<QuoteResponse>("/api/quote?location=London&offset=1");

        Assert.NotNull(body);
        Assert.NotNull(body!.Weather.Date);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), body.Weather.Date);
        Assert.Single(body.Quotes);
    }

    [Fact]
    public async Task GetQuote_WithOffsetOutOfRange_ReturnsBadRequest()
    {
        var client = BuildClient();
        var response = await client.GetAsync("/api/quote?location=London&offset=5");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- Stubs ---

    private sealed class StubWeatherService : IWeatherService
    {
        public Task<WeatherData> GetCurrentWeatherAsync(string location) =>
            Task.FromResult(new WeatherData(
                Location: "London, UK",
                Description: "overcast",
                TemperatureCelsius: 14,
                HumidityPercent: 80,
                WindSpeedMps: 4,
                Condition: "Cloudy"
            ));

        public Task<WeatherData> GetForecastWeatherAsync(string location, int offsetDays) =>
            Task.FromResult(new WeatherData(
                Location: "London, UK",
                Description: "light rain",
                TemperatureCelsius: 11,
                HumidityPercent: 80,
                WindSpeedMps: 5,
                Condition: "Rain",
                Date: DateOnly.FromDateTime(DateTime.Today.AddDays(offsetDays))
            ));
    }

    private sealed class StubQuoteSearchService : IQuoteSearchService
    {
        public Task<IReadOnlyList<QuoteResult>> SearchAsync(
            string weatherDescription, double temperatureCelsius, string condition, int limit = 3) =>
            Task.FromResult<IReadOnlyList<QuoteResult>>(
            [
                new QuoteResult(
                    Text: "It was a dark and stormy night.",
                    Book: "Paul Clifford",
                    Author: "Edward Bulwer-Lytton",
                    Score: 0.92,
                    Era: "Victorian",
                    Genre: "novel",
                    Language: "en")
            ]);
    }

    private sealed class ThrowingWeatherService : IWeatherService
    {
        public Task<WeatherData> GetCurrentWeatherAsync(string location) =>
            throw new ArgumentException($"Location '{location}' not found.");

        public Task<WeatherData> GetForecastWeatherAsync(string location, int offsetDays) =>
            throw new ArgumentException($"Location '{location}' not found.");
    }

    private sealed record QuoteResponse(WeatherData Weather, string WeatherProse, QuoteResult[] Quotes);
    // WeatherData already includes Date (DateOnly?) from the model
    private sealed record ErrorResponse(string Error);
}
