using FeedR.Feeds.Quotes.Pricing.Models;
using Grpc.Core;
using System.Collections.Concurrent;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal sealed class PricingGrpcService : PricingFeed.PricingFeedBase
    {
        private readonly IPricingGenerator _generator;
        private readonly BlockingCollection<CurrencyPair> _currencyPairs = new();
        private readonly ILogger<PricingGrpcService> _logger;

        public PricingGrpcService(IPricingGenerator generator, ILogger<PricingGrpcService> logger)
        {
            _generator = generator;
            _logger = logger;
        }

        public override Task<GetSymbolsResponse> GetSymbols(GetSymbolsRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetSymbolsResponse()
            {
                Symbols =
                {
                    _generator.GetSymbols()
                }
            });
        }

        public override async Task SubscribePricing(PricingRequest request, 
            IServerStreamWriter<PricingResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Started client streaming.");
            _generator.PricingUpdated += OnPricingUpdated;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                if (!_currencyPairs.TryTake(out var currencyPair))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(request.Symbol) && request.Symbol != currencyPair.Symbol)
                {
                    continue;
                }

                await responseStream.WriteAsync(new PricingResponse
                {
                    Symbol = currencyPair.Symbol,
                    Timestamp = currencyPair.Timestamp,
                    Value = (int)(100 * currencyPair.Value)
                });
            }

            _generator.PricingUpdated -= OnPricingUpdated;
            _logger.LogInformation("Stopped client streaming.");

            void OnPricingUpdated(object? sender, CurrencyPair currencyPair)
            {
                _currencyPairs.TryAdd(currencyPair);
            }
        }
    }
}
