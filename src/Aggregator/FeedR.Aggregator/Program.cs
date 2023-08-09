using FeedR.Aggregator.Services;
using FeedR.Shared.Redis;
using FeedR.Shared.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Messaging;
using FeedR.Shared.Pulsar;
using FeedR.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming() // The order matters here
    .AddHostedService<PricingStreamBackgroundService>()
    .AddHostedService<WeatherStreamBackgroundService>()
    .AddSerialization()
    .AddMessaging()
    .AddPulsar() // The order matters here
    .AddSingleton<IPricingHandler, PricingHandler>();

var app = builder.Build();

app.UseCorrelationId();

app.MapGet("/", async (ctx) =>
{
    var requestId = ctx.Request.Headers["x-request-id"];

    await ctx.Response.WriteAsync($"FeedR Aggregator. Request ID: {requestId}.");
});

app.Run();