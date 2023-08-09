using FeedR.Feeds.Quotes.Pricing.Requests;
using FeedR.Feeds.Quotes.Pricing.Services;
using FeedR.Shared.Messaging;
using FeedR.Shared.Observability;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming() // The order matters
    .AddSingleton<IPricingGenerator, PricingGenerator>()
    .AddSingleton<PricingRequestsChannel>()
    .AddHostedService<PricingBackgroundService>()
    .AddSerialization()
    .AddGrpc();

var app = builder.Build();

app.UseCorrelationId();

app.MapGrpcService<PricingGrpcService>();

app.MapGet("/", async (ctx) =>
{
    await ctx.Response.WriteAsync("FeedR Quotes feed.");
});

app.MapPost("/pricing/start", async (HttpContext context, PricingRequestsChannel channel) =>
{
    var correlationId = context.GetCorrelationId();

    await channel.Requests.Writer.WriteAsync(new StartPricing());
    return Results.Accepted();
});

app.MapPost("/pricing/stop", async (PricingRequestsChannel channel) =>
{
    await channel.Requests.Writer.WriteAsync(new StopPricing());
    return Results.Accepted();
});

app.Run();