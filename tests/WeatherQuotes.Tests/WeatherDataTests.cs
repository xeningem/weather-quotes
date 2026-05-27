using WeatherQuotes.Api.Models;

namespace WeatherQuotes.Tests;

public class WeatherDataTests
{
    private static WeatherData Make(string condition, double temp) =>
        new("TestCity", "test description", temp, 60, 3.0, condition);

    [Fact]
    public void ToNaturalLanguage_AlwaysIncludesLocation()
    {
        var prose = Make("Clear", 15).ToNaturalLanguage();
        Assert.Contains("TestCity", prose);
    }

    [Fact]
    public void ToNaturalLanguage_IncludesRoundedTemperature()
    {
        var prose = Make("Clear", 17.7).ToNaturalLanguage();
        Assert.Contains("18°C", prose);
    }

    [Theory]
    [InlineData("Thunderstorm", 10,  "stormy",     "cold")]
    [InlineData("Thunderstorm", 20,  "summer",     "lightning")]
    [InlineData("Rain",          3,  "winter",     "grey")]
    [InlineData("Rain",         12,  "autumn",     "raw")]
    [InlineData("Rain",         20,  "warm",       "overcast")]
    [InlineData("Snow",        -15,  "bitterly",   "frozen")]
    [InlineData("Snow",         -5,  "snow",       "white")]
    [InlineData("Fog",           8,  "fog",        "mist")]
    [InlineData("Drizzle",      12,  "drizzl",     "grey")]
    [InlineData("Clear",        30,  "blazing",    "sunshine")]
    [InlineData("Clear",        20,  "beautiful",  "sunshine")]
    [InlineData("Clear",         8,  "crisp",      "cloudless")]
    [InlineData("Clear",        -5,  "freezing",   "frost")]
    [InlineData("Cloudy",        5,  "overcast",   "grey")]
    [InlineData("Cloudy",       15,  "overcast",   "clouds")]
    public void ToNaturalLanguage_ContainsExpectedPhrases(
        string condition, double temp, string phrase1, string phrase2)
    {
        var prose = Make(condition, temp).ToNaturalLanguage();
        Assert.Contains(phrase1, prose, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(phrase2, prose, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToNaturalLanguage_UnknownConditionFallsBackToDescription()
    {
        var weather = new WeatherData("TestCity", "mysterious haze", 10, 50, 2.0, "Haze");
        var prose = weather.ToNaturalLanguage();
        Assert.Contains("Haze", prose);
        Assert.Contains("mysterious haze", prose);
    }
}
