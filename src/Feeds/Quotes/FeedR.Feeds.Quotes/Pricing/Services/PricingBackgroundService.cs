using FeedR.Feeds.Quotes.Requests;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal class PricingBackgroundService : BackgroundService
    {
        private readonly IPricingGenerator _generator;
        private readonly PricingRequestsChannel _channel;

        public PricingBackgroundService(IPricingGenerator generator, PricingRequestsChannel channel)
        {
            _generator = generator;
            _channel = channel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var request in _channel.Requests.Reader.ReadAllAsync())
            {
                var _ = request switch
                {
                    StartPricing => _generator.StartAsync(),
                    StopPricing => _generator.StopAsync(),
                    _ => Task.CompletedTask
                };
            }
        }
    }
}
