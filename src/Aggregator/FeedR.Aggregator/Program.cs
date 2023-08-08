using FeedR.Aggregator.Services;
using FeedR.Shared.Redis;
using FeedR.Shared.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Redis.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming() // The order matters
    .AddHostedService<PricingStreamBackgroundService>()
    .AddSerialization();

var app = builder.Build();

app.MapGet("/", async (ctx) =>
{
    var requestId = ctx.Request.Headers["x-request-id"];

    await ctx.Response.WriteAsync($"FeedR Aggregator. Request ID: {requestId}.");
});

app.Run();