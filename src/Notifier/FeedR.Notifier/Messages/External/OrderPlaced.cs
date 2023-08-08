using FeedR.Shared.Messaging;

namespace FeedR.Notifier.Messages.External
{
    internal record OrderPlaced(string OrderId, string Symbol) : IMessage;
}
