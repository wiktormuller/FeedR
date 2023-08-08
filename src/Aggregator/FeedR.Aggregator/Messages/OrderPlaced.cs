using FeedR.Shared.Messaging;

namespace FeedR.Aggregator.Messages
{
    internal record OrderPlaced(string OrderId, string Symbol) : IMessage;
}
