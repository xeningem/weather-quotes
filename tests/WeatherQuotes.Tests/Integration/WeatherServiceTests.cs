using System.Net;
using System.Text;
using WeatherQuotes.Api.Services;

namespace WeatherQuotes.Tests.Integration;

public class WeatherServiceTests
{
    private static WeatherService BuildService(params (string url, string body)[] responses)
    {
        var handler = new SequencedFakeHandler(responses);
        return new WeatherService(new HttpClient(handler));
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_ParsesResponseCorrectly()
    {
        var service = BuildService(
            ("geocoding", """{"results":[{"name":"London","country":"United Kingdom","latitude":51.5,"longitude":-0.1}]}"""),
            ("forecast", """{"current":{"temperature_2m":14.5,"relative_humidity_2m":72,"wind_speed_10m":3.2,"weather_code":3}}""")
        );

        var result = await service.GetCurrentWeatherAsync("London");

        Assert.Equal("London, United Kingdom", result.Location);
        Assert.Equal(14.5, result.TemperatureCelsius);
        Assert.Equal(72, result.HumidityPercent);
        Assert.Equal(3.2, result.WindSpeedMps);
        Assert.Equal("Cloudy", result.Condition);
        Assert.Equal("overcast", result.Description);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_LocationWithoutCountry_UsesJustName()
    {
        var service = BuildService(
            ("geocoding", """{"results":[{"name":"Springfield","latitude":39.8,"longitude":-89.6}]}"""),
            ("forecast", """{"current":{"temperature_2m":20.0,"relative_humidity_2m":50,"wind_speed_10m":2.0,"weather_code":1}}""")
        );

        var result = await service.GetCurrentWeatherAsync("Springfield");

        Assert.Equal("Springfield", result.Location);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_LocationNotFound_Throws()
    {
        var service = BuildService(
            ("geocoding", """{"results":[]}""")
        );

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetCurrentWeatherAsync("Atlantis"));
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_GeocodeHttpError_Throws()
    {
        var handler = new ErrorFakeHandler(HttpStatusCode.ServiceUnavailable);
        var service = new WeatherService(new HttpClient(handler));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            service.GetCurrentWeatherAsync("London"));
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WeatherHttpError_Throws()
    {
        var handler = new PartialErrorHandler(
            geocodeBody: """{"results":[{"name":"London","country":"UK","latitude":51.5,"longitude":-0.1}]}""",
            weatherStatus: HttpStatusCode.InternalServerError
        );
        var service = new WeatherService(new HttpClient(handler));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            service.GetCurrentWeatherAsync("London"));
    }

    private sealed class SequencedFakeHandler(params (string tag, string body)[] responses) : HttpMessageHandler
    {
        private int _index;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var (_, body) = responses[_index++];
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class ErrorFakeHandler(HttpStatusCode status) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(new HttpResponseMessage(status));
    }

    private sealed class PartialErrorHandler(string geocodeBody, HttpStatusCode weatherStatus) : HttpMessageHandler
    {
        private bool _geocodeDone;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (!_geocodeDone)
            {
                _geocodeDone = true;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(geocodeBody, Encoding.UTF8, "application/json")
                });
            }
            return Task.FromResult(new HttpResponseMessage(weatherStatus));
        }
    }
}
