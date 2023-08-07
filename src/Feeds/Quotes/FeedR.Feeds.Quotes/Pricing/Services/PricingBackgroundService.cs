namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal class PricingBackgroundService : IHostedService
    {
        private readonly IPricingGenerator _generator;

        public PricingBackgroundService(IPricingGenerator generator)
        {
            _generator = generator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ = _generator.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _generator.StopAsync();
        }
    }
}
