namespace FeedR.Shared.Streaming
{
    internal sealed class DefaultStreamSubscriber : IStreamSubscriber
    {
        public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class
        {
            return Task.CompletedTask;
        }
    }
}
