using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Feeds.Quotes.Pricing.Services;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming() // The order matters
    .AddSingleton<IPricingGenerator, PricingGenerator>()
    .AddSingleton<PricingRequestsChannel>()
    .AddHostedService<PricingBackgroundService>()
    .AddSerialization()
    .AddGrpc();

var app = builder.Build();

app.MapGrpcService<PricingGrpcService>();

app.MapGet("/", async (ctx) =>
{
    await ctx.Response.WriteAsync("FeedR Quotes feed.");
});

app.MapPost("/pricing/start", async (PricingRequestsChannel channel) =>
{
    await channel.Requests.Writer.WriteAsync(new StartPricing());
    return Results.Accepted();
});

app.MapPost("/pricing/stop", async (PricingRequestsChannel channel) =>
{
    await channel.Requests.Writer.WriteAsync(new StopPricing());
    return Results.Accepted();
});

app.Run();