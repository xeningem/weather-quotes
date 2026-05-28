using WeatherQuotes.Api.Services;

namespace WeatherQuotes.Tests;

public class WmoCodeTests
{
    [Theory]
    [InlineData(0,   "Clear",        "clear sky")]
    [InlineData(1,   "Clear",        "mainly clear")]
    [InlineData(2,   "Cloudy",       "partly cloudy")]
    [InlineData(3,   "Cloudy",       "overcast")]
    [InlineData(45,  "Fog",          "foggy")]
    [InlineData(48,  "Fog",          "foggy")]
    [InlineData(51,  "Drizzle",      "light drizzle")]
    [InlineData(53,  "Drizzle",      "moderate drizzle")]
    [InlineData(55,  "Drizzle",      "dense drizzle")]
    [InlineData(61,  "Rain",         "slight rain")]
    [InlineData(63,  "Rain",         "moderate rain")]
    [InlineData(65,  "Rain",         "heavy rain")]
    [InlineData(71,  "Snow",         "slight snowfall")]
    [InlineData(73,  "Snow",         "moderate snowfall")]
    [InlineData(75,  "Snow",         "heavy snowfall")]
    [InlineData(77,  "Snow",         "snow grains")]
    [InlineData(80,  "Rain",         "slight rain showers")]
    [InlineData(81,  "Rain",         "moderate rain showers")]
    [InlineData(82,  "Rain",         "violent rain showers")]
    [InlineData(85,  "Snow",         "snow showers")]
    [InlineData(86,  "Snow",         "snow showers")]
    [InlineData(95,  "Thunderstorm", "thunderstorm")]
    [InlineData(96,  "Thunderstorm", "thunderstorm with hail")]
    [InlineData(99,  "Thunderstorm", "thunderstorm with hail")]
    [InlineData(999, "Unknown",      "unknown conditions")]
    public void Describe_ReturnsCorrectPair(int code, string expectedCondition, string expectedDescription)
    {
        var (condition, description) = WmoCode.Describe(code);
        Assert.Equal(expectedCondition, condition);
        Assert.Equal(expectedDescription, description);
    }
}
