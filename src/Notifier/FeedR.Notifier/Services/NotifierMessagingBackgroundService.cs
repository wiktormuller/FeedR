using FeedR.Notifier.Messages.External;
using FeedR.Shared.Messaging;

namespace FeedR.Notifier.Services
{
    internal sealed class NotifierMessagingBackgroundService : BackgroundService
    {
        private readonly IMessageSubscriber _messageSubscriber;
        private readonly ILogger<NotifierMessagingBackgroundService> _logger;

        public NotifierMessagingBackgroundService(IMessageSubscriber messageSubscriber,
            ILogger<NotifierMessagingBackgroundService> logger)
        {
            _messageSubscriber = messageSubscriber;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageSubscriber.SubscribeAsync<OrderPlaced>("orders", message =>
            {
                _logger.LogInformation($"Order with ID: '{message.Message.OrderId}' for symbol: " +
                    $"'{message.Message.Symbol}' has been placed. The CorrelationId: '{message.CorrelationId}'");
            });

            return Task.CompletedTask;
        }
    }
}
