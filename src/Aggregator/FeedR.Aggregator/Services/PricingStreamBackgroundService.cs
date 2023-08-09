using FeedR.Aggregator.Models;
using FeedR.Shared.Streaming;

namespace FeedR.Aggregator.Services
{
    internal sealed class PricingStreamBackgroundService : BackgroundService
    {
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly ILogger<PricingStreamBackgroundService> _logger;
        private readonly IPricingHandler pricingHandler;

        public PricingStreamBackgroundService(IStreamSubscriber streamSubscriber,
            ILogger<PricingStreamBackgroundService> logger,
            IPricingHandler pricingHandler)
        {
            _streamSubscriber = streamSubscriber;
            _logger = logger;
            this.pricingHandler = pricingHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<CurrencyPair>("pricing", async (currencyPair) =>
            {
                _logger.LogInformation($"Pricing '{currencyPair.Symbol}' = {currencyPair.Value:F}, " +
                    $"timestamp: {currencyPair.Timestamp}");

                _ = pricingHandler.HandleAsync(currencyPair);
            });
        }
    }
}
