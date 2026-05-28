using WeatherQuotes.Api.Services;

namespace WeatherQuotes.Tests.Integration;

public class QuoteFilterTests
{
    // --- IsTemperatureCompatible ---

    [Theory]
    [InlineData("bitterly cold wind swept the plain", 20, false)]
    [InlineData("the frost glittered on the windowpane", 20, false)]
    [InlineData("cold air filled the room", 20, false)]
    [InlineData("a warm summer evening on the terrace", 20, true)]
    [InlineData("the rain fell steadily through the night", 20, true)]
    public void IsTemperatureCompatible_WarmTemperature_ExcludesColdTerms(string text, double temp, bool expected)
        => Assert.Equal(expected, QuoteSearchService.IsTemperatureCompatible(text, temp));

    [Theory]
    [InlineData("the sweltering heat was unbearable", 3, false)]
    [InlineData("scorching sun baked the earth", 3, false)]
    [InlineData("sultry air clung to the skin", 3, false)]
    [InlineData("she wrapped her coat against the chill", 3, true)]
    [InlineData("snow lay thick upon the ground", 3, true)]
    public void IsTemperatureCompatible_ColdTemperature_ExcludesHotTerms(string text, double temp, bool expected)
        => Assert.Equal(expected, QuoteSearchService.IsTemperatureCompatible(text, temp));

    [Fact]
    public void IsTemperatureCompatible_MildTemperature_AcceptsAnything()
    {
        Assert.True(QuoteSearchService.IsTemperatureCompatible("sweltering heat", 12));
        Assert.True(QuoteSearchService.IsTemperatureCompatible("bitter frost", 12));
    }

    // --- IsConditionCompatible ---

    [Theory]
    [InlineData("Thunderstorm", "blazing sun overhead", false)]
    [InlineData("Rain", "not a cloud in the sky", false)]
    [InlineData("Drizzle", "brilliant sunshine all day", false)]
    [InlineData("Thunderstorm", "the rain hammered the roof", true)]
    [InlineData("Rain", "grey clouds and puddles", true)]
    public void IsConditionCompatible_WetConditions_ExcludesSunTerms(string condition, string text, bool expected)
        => Assert.Equal(expected, QuoteSearchService.IsConditionCompatible(text, condition));

    [Theory]
    [InlineData("Clear", "thunder rolled across the hills", false)]
    [InlineData("Clear", "the downpour soaked everything", false)]
    [InlineData("Clear", "a blizzard blocked the road", false)]
    [InlineData("Clear", "the sky was bright and open", true)]
    public void IsConditionCompatible_ClearCondition_ExcludesStormTerms(string condition, string text, bool expected)
        => Assert.Equal(expected, QuoteSearchService.IsConditionCompatible(text, condition));

    [Theory]
    [InlineData("Snow", "sweltering heat in the valley", false)]
    [InlineData("Snow", "summer heat lay heavy", false)]
    [InlineData("Snow", "snow settled on the branches", true)]
    public void IsConditionCompatible_SnowCondition_ExcludesHeatTerms(string condition, string text, bool expected)
        => Assert.Equal(expected, QuoteSearchService.IsConditionCompatible(text, condition));

    [Theory]
    [InlineData("Fog", "anything goes in fog")]
    [InlineData("Cloudy", "even hot text is fine in cloudy")]
    [InlineData("Unknown", "unknown accepts everything")]
    public void IsConditionCompatible_OtherConditions_AlwaysTrue(string condition, string text)
        => Assert.True(QuoteSearchService.IsConditionCompatible(text, condition));
}
