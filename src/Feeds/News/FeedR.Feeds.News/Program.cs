var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor();

var app = builder.Build();

app.MapGet("/", () => "FeedR News feed");

app.Run();