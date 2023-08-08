using FeedR.Aggregator.Services;
using FeedR.Shared.Redis;
using FeedR.Shared.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming() // The order matters
    .AddHostedService<PricingStreamBackgroundService>()
    .AddHostedService<WeatherStreamBackgroundService>()
    .AddSerialization()
    .AddMessaging()
    .AddSingleton<IPricingHandler, PricingHandler>();

var app = builder.Build();

app.MapGet("/", async (ctx) =>
{
    var requestId = ctx.Request.Headers["x-request-id"];

    await ctx.Response.WriteAsync($"FeedR Aggregator. Request ID: {requestId}.");
});

app.Run();