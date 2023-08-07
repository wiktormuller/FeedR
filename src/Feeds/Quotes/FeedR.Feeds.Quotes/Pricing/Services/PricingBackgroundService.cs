using FeedR.Feeds.Quotes.Requests;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal class PricingBackgroundService : BackgroundService
    {
        private int _runningStatus; // 0 - disabled or 1 - already running
        private readonly IPricingGenerator _generator;
        private readonly PricingRequestsChannel _channel;
        private readonly ILogger<PricingBackgroundService> _logger;

        public PricingBackgroundService(IPricingGenerator generator,
            PricingRequestsChannel channel, 
            ILogger<PricingBackgroundService> logger)
        {
            _generator = generator;
            _channel = channel;
            _logger = logger;
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
            await _generator.StartAsync();
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
