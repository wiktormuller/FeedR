using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace FeedR.Shared.HTTP
{
    public static class Extensions
    {
        public static IServiceCollection AddHttpApiClient<TInterface, TClient>(this IServiceCollection services)
            where TInterface : class where TClient : class, TInterface
        {
            services
                .AddTransient<CorrelationIdMessageHandler>()
                .AddHttpClient<TInterface, TClient>()
                .AddHttpMessageHandler<CorrelationIdMessageHandler>()
                .AddPolicyHandler(GetRetryPolicy());

            return services;

            static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(message => message.StatusCode == System.Net.HttpStatusCode.TooManyRequests 
                        || message.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(100, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));
            }
        }
    }
}
