using FeedR.Aggregator.Messages;
using FeedR.Aggregator.Models;
using FeedR.Shared.Messaging;

namespace FeedR.Aggregator.Services
{
    internal class PricingHandler : IPricingHandler
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<PricingHandler> _logger;
        private int _counter;

        public PricingHandler(IMessagePublisher messagePublisher, 
            ILogger<PricingHandler> logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task HandleAsync(CurrencyPair currencypair)
        {
            // TODO: Implement some actual business logic
            if (ShouldPlaceOrder())
            {
                var orderId = Guid.NewGuid().ToString("N");
                _logger.LogInformation($"Order with ID: {orderId} has been placed for symbol: {currencypair.Symbol}.");

                var integrationEvent = new OrderPlaced(orderId, currencypair.Symbol);

                await _messagePublisher.PublishAsync("orders", integrationEvent);
            }
        }

        private bool ShouldPlaceOrder()
            => Interlocked.Increment(ref _counter) % 10 == 0;
    }
}
