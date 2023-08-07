using FeedR.Feeds.Quotes.Pricing.Services;
using FeedR.Feeds.Quotes.Requests;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IPricingGenerator, PricingGenerator>()
    .AddSingleton<PricingRequestsChannel>()
    .AddHostedService<PricingBackgroundService>();

var app = builder.Build();

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