using FeedR.Feeds.Weather.Models;
using System.Text.Json.Serialization;

namespace FeedR.Feeds.Weather.Services
{
    internal sealed class WeatherFeed : IWeatherFeed
    {
        private readonly HttpClient _httpClient;

        // TODO: Move this values to appsettings
        private const string ApiKey = "secret ";
        private const string ApiUrl = "https://api.weatherapi.com/v1/current.json";

        public WeatherFeed(HttpClient client)
        {
            _httpClient = client;
        }

        public async IAsyncEnumerable<WeatherData> SubscribeAsync(string location, CancellationToken cancellationToken)
        {
            var url = $"{ApiUrl}?key={ApiKey}&q={location}&aqi=no";

            while (!cancellationToken.IsCancellationRequested)
            {
                var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(url, cancellationToken);
                
                if (response is null)
                {
                    continue;
                }

                yield return new WeatherData($"{response.Location.Name}, {response.Location.Country}",
                    response.Current.TempC, response.Current.Humidity, response.Current.WindKph, response.Current.Condition.Text);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private record WeatherApiResponse(Location Location, Weather Current);

        private record Location(string Name, string Country);

        private record Condition(string Text);

        private record Weather([property: JsonPropertyName("temp_c")]double TempC, double Humidity, Condition Condition,
            [property: JsonPropertyName("wind_kph")] double WindKph);
    }
}
