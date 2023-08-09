using FeedR.Feeds.News.Messages;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Streaming;
using FeedR.Shared.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddStreaming()
    .AddRedis(builder.Configuration)
    .AddRedisStreaming()
    .AddSerialization();

var app = builder.Build();

app.MapGet("/", () => "FeedR News Feed");

app.MapPost("/news", async(PublishNews news, IStreamPublisher streamPublisher) =>
{
    // TODO: Handle the published news
    var @event = new NewsPublished(news.Title, news.Category);

    //Task.Run(() => Task.Delay(10000)).ContinueWith(t => streamPublisher.PublishAsync("news", @event)); // Simulating long processing
    await streamPublisher.PublishAsync("news", @event);

    return Results.Accepted();
});

app.Run();