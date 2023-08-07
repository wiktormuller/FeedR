var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (ctx) =>
{
    var requestId = ctx.Request.Headers["x-request-id"];

    await ctx.Response.WriteAsync($"FeedR Aggregator. Request ID: {requestId}.");
});

app.Run();