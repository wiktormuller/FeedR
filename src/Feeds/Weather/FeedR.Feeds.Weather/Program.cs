using FeedR.Feeds.Weather.Services;
using FeedR.Shared.HTTP;
using FeedR.Shared.Redis;
using FeedR.Shared.Redis.Streaming;
using FeedR.Shared.Serialization;
using FeedR.Shared.Streaming;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient()
    .AddHttpApiClient<IWeatherFeed, WeatherFeed>()
    .AddHostedService<WeatherBackgroundService>()
    .AddRedis(builder.Configuration)
    .AddStreaming()
    .AddRedisStreaming()
    .AddSerialization(); // The order matters;

var app = builder.Build();

app.MapGet("/", () => "FeedR Weather feed");

app.Run();