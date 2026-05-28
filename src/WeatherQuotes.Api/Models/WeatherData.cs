namespace WeatherQuotes.Api.Models;

public record WeatherData(
    string Location,
    string Description,
    double TemperatureCelsius,
    int HumidityPercent,
    double WindSpeedMps,
    string Condition,
    DateOnly? Date = null
)
{
    public string ToNaturalLanguage()
    {
        var temp = TemperatureCelsius;
        var prose = Condition switch
        {
            "Thunderstorm" when temp < 15 =>
                $"A dark, stormy sky with thunder and lightning over {Location}. " +
                $"Rain pours down, the air is cold and damp at {temp:F0}°C. " +
                $"Wind howls, everything is grey and turbulent.",
            "Thunderstorm" =>
                $"A violent summer thunderstorm over {Location}. " +
                $"Thunder crashes and lightning flashes. Warm rain at {temp:F0}°C, " +
                $"heavy and oppressive before the storm breaks.",
            "Rain" when temp < 5 =>
                $"A cold, wet winter day in {Location}. " +
                $"Rain falls from a grey sky at {temp:F0}°C. " +
                $"Everything is damp and cheerless, puddles on the ground.",
            "Rain" when temp < 15 =>
                $"A grey autumn day in {Location}. {Description}. " +
                $"The rain is steady and cold at {temp:F0}°C, " +
                $"the streets are wet, the air raw and damp.",
            "Rain" =>
                $"A warm rainy day in {Location}. {Description}. " +
                $"Soft rain falls from an overcast sky at {temp:F0}°C, " +
                $"the air heavy and humid.",
            "Snow" when temp < -10 =>
                $"A bitterly cold blizzard in {Location}. Snow and ice everywhere at {temp:F0}°C. " +
                $"The wind is fierce, the world white and frozen.",
            "Snow" =>
                $"Snow falls quietly over {Location} at {temp:F0}°C. " +
                $"The ground is blanketed in white, the air still and cold.",
            "Fog" =>
                $"A thick fog shrouds {Location} at {temp:F0}°C. " +
                $"Visibility is low, shapes loom out of the grey mist. " +
                $"Everything is hushed and mysterious.",
            "Drizzle" =>
                $"A misty, drizzly day in {Location} at {temp:F0}°C. " +
                $"Fine rain hangs in the air, dampening everything. " +
                $"The sky is a dull grey.",
            "Clear" when temp >= 25 =>
                $"A blazing hot clear day in {Location}. Brilliant sunshine at {temp:F0}°C, " +
                $"not a cloud in the sky. The heat shimmers off the ground.",
            "Clear" when temp >= 15 =>
                $"A beautiful clear day in {Location}. Bright sunshine and blue sky at {temp:F0}°C. " +
                $"The air is fresh and warm, a light breeze.",
            "Clear" when temp >= 5 =>
                $"A crisp, clear day in {Location} at {temp:F0}°C. " +
                $"The sky is bright and cloudless, but the air is cold and clean.",
            "Clear" =>
                $"A freezing clear day in {Location} at {temp:F0}°C. " +
                $"The sky is hard and bright, frost on the ground, bitterly cold.",
            "Cloudy" when temp < 10 =>
                $"A cold overcast day in {Location} at {temp:F0}°C. " +
                $"Low grey clouds hang heavy over the city. Dull and cheerless.",
            "Cloudy" =>
                $"An overcast day in {Location} at {temp:F0}°C. " +
                $"Grey clouds cover the sky. The light is flat, the air mild.",
            _ =>
                $"{Condition} weather in {Location}. {Description}. " +
                $"{temp:F0}°C."
        };
        return prose;
    }
}

public record QuoteResult(
    string Text,
    string Book,
    string Author,
    double Score,
    string? Era = null,
    string? Genre = null,
    string? Language = null
);
