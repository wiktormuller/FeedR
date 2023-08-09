using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace FeedR.Shared.Observability
{
    public static class Extensions
    {
        private const string CorrelationIdKey = "correlation-id";

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
            => app.Use(async (ctx, next) =>
            {
                if (!ctx.Request.Headers.TryGetValue(CorrelationIdKey, out var correlationId))
                {
                    // We are doing it, because maybe there is not communication from Gateway,
                    // but it's microservice to microservice communication
                    correlationId = Guid.NewGuid().ToString("N");
                }

                ctx.Items[CorrelationIdKey] = correlationId.ToString(); // We want to keep the correlationId in context items dictionary
                
                await next();
            });

        public static string? GetCorrelationId(this HttpContext context)
            => context.Items.TryGetValue(CorrelationIdKey, out var correlationId) 
                ? correlationId as string 
                : null;

        public static void AddCorrelationId(this HttpRequestHeaders headers, string correlationId)
            => headers.TryAddWithoutValidation(CorrelationIdKey, correlationId);
    }
}
