using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;
using StackExchange.Redis;

namespace FeedR.Shared.Redis.Streaming
{
    internal sealed class RedisStreamSubscriber : IStreamSubscriber
    {
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;

        public RedisStreamSubscriber(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _serializer = serializer;
        }

        public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class
        => _subscriber.SubscribeAsync(topic, (redisChannel, redisValue) =>
        {
            var payload = _serializer.Deserialize<T>(redisValue);
            if (payload is null)
            {
                return;
            }

            handler(payload);
        });
    }
}
