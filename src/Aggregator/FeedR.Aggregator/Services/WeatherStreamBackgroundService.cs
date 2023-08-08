using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services
{
    internal sealed class WeatherStreamBackgroundService : BackgroundService
    {
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly ILogger<WeatherStreamBackgroundService> _logger;

        public WeatherStreamBackgroundService(IStreamSubscriber streamSubscriber, 
            ILogger<WeatherStreamBackgroundService> logger)
        {
            _streamSubscriber = streamSubscriber;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<WeatherData>("weather", (data) =>
            {
                _logger.LogInformation($"{data.Location}: {data.Temperature}°C, {data.Humidity}%, {data.Wind} km/h, [{data.Condition}]");
            });
        }

        private record WeatherData(string Location, double Temperature, double Humidity, double Wind, string Condition);
    }
}
