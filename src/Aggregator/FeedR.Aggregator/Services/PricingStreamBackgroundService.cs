using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services
{
    internal sealed class PricingStreamBackgroundService : BackgroundService
    {
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly ILogger<PricingStreamBackgroundService> _logger;

        public PricingStreamBackgroundService(IStreamSubscriber streamSubscriber, 
            ILogger<PricingStreamBackgroundService> logger)
        {
            _streamSubscriber = streamSubscriber;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<CurrencyPair>("pricing", (currencyPair) =>
            {
                _logger.LogInformation($"Pricing '{currencyPair.Symbol}' = {currencyPair.Value:F}, " +
                    $"timestamp: {currencyPair.Timestamp}");
            });
        }

        private record CurrencyPair(string Symbol, decimal Value, long Timestamp);
    }
}
