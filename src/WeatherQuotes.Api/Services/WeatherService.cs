using System.Text.Json;
using WeatherQuotes.Api.Models;

namespace WeatherQuotes.Api.Services;

public class WeatherService(HttpClient http)
{

    public async Task<WeatherData> GetCurrentWeatherAsync(string location)
    {
        var (lat, lon, resolvedName) = await GeocodeAsync(location);

        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  $"&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code" +
                  $"&wind_speed_unit=ms";

        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var current = doc.RootElement.GetProperty("current");

        var code = current.GetProperty("weather_code").GetInt32();
        var (condition, description) = WmoCode.Describe(code);

        return new WeatherData(
            Location: resolvedName,
            Description: description,
            TemperatureCelsius: current.GetProperty("temperature_2m").GetDouble(),
            HumidityPercent: current.GetProperty("relative_humidity_2m").GetInt32(),
            WindSpeedMps: current.GetProperty("wind_speed_10m").GetDouble(),
            Condition: condition
        );
    }

    private async Task<(double lat, double lon, string name)> GeocodeAsync(string location)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1&language=en";
        var response = await http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            throw new ArgumentException($"Location '{location}' not found.");

        var first = results[0];
        var name = first.GetProperty("name").GetString() ?? location;
        if (first.TryGetProperty("country", out var country))
            name += $", {country.GetString()}";

        return (first.GetProperty("latitude").GetDouble(), first.GetProperty("longitude").GetDouble(), name);
    }
}

public static class WmoCode
{
    public static (string condition, string description) Describe(int code) => code switch
    {
        0 => ("Clear", "clear sky"),
        1 => ("Clear", "mainly clear"),
        2 => ("Cloudy", "partly cloudy"),
        3 => ("Cloudy", "overcast"),
        45 or 48 => ("Fog", "foggy"),
        51 => ("Drizzle", "light drizzle"),
        53 => ("Drizzle", "moderate drizzle"),
        55 => ("Drizzle", "dense drizzle"),
        61 => ("Rain", "slight rain"),
        63 => ("Rain", "moderate rain"),
        65 => ("Rain", "heavy rain"),
        71 => ("Snow", "slight snowfall"),
        73 => ("Snow", "moderate snowfall"),
        75 => ("Snow", "heavy snowfall"),
        77 => ("Snow", "snow grains"),
        80 => ("Rain", "slight rain showers"),
        81 => ("Rain", "moderate rain showers"),
        82 => ("Rain", "violent rain showers"),
        85 or 86 => ("Snow", "snow showers"),
        95 => ("Thunderstorm", "thunderstorm"),
        96 or 99 => ("Thunderstorm", "thunderstorm with hail"),
        _ => ("Unknown", "unknown conditions")
    };
}
