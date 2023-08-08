using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Shared.Streaming;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal class PricingBackgroundService : BackgroundService
    {
        private int _runningStatus; // 0 - disabled or 1 - already running
        private readonly IPricingGenerator _generator;
        private readonly PricingRequestsChannel _channel;
        private readonly ILogger<PricingBackgroundService> _logger;
        private readonly IStreamPublisher _streamPublisher;

        public PricingBackgroundService(IPricingGenerator generator,
            PricingRequestsChannel channel,
            ILogger<PricingBackgroundService> logger,
            IStreamPublisher streamPublisher)
        {
            _generator = generator;
            _channel = channel;
            _logger = logger;
            _streamPublisher = streamPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pricing background service has started");
            await foreach (var request in _channel.Requests.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Pricing background service has received the request: {request.GetType().Name}.");

                var _ = request switch
                {
                    StartPricing => StartGeneratorAsync(),
                    StopPricing => StopGeneratorAsync(),
                    _ => Task.CompletedTask
                };
            }

            _logger.LogInformation("Pricing background service has started");
        }

        // The Interlocked.Exchange makes our two operations (comparing and setting - incrementing) atomic 
        private async Task StartGeneratorAsync() // Making start/stopping our generator thread-safe, and don't allow many threads to start/stop in parallel
        {
            if (Interlocked.Exchange(ref _runningStatus, 1) == 1) // We want to exchange (set) to 1 the variable, but if it's actually 1 then do nothing
            {
                _logger.LogInformation("Pricing generator is already running.");
                return;
            }

            await foreach (var currencyPair in _generator.StartAsync())
            {
                _logger.LogInformation("Publishing the currency pair...");
                await _streamPublisher.PublishAsync("pricing", currencyPair);
            }
        }

        private async Task StopGeneratorAsync()
        {
            if (Interlocked.Exchange(ref _runningStatus, 0) == 0) // We want to exchange (set) to 0 the variable, but if it's actually 0 then do nothing
            {
                _logger.LogInformation("Pricing generator is not running.");
                return;
            }
            await _generator.StopAsync();
        }
    }
}
