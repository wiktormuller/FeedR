﻿using FeedR.Feeds.Quotes.Pricing.Models;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    internal sealed class PricingGenerator : IPricingGenerator
    {
        private readonly Dictionary<string, decimal> _currencyPairs = new()
        {
            ["EURUSD"] = 1.13M,
            ["EURGBP"] = 0.85M,
            ["EURCHF"] = 1.04M,
            ["EURPLN"] = 4.62M,
        };
        private readonly ILogger<IPricingGenerator> _logger;
        private bool _isRunning;
        private readonly Random _random = new Random();

        public event EventHandler<CurrencyPair>? PricingUpdated;

        public PricingGenerator(ILogger<IPricingGenerator> logger)
        {
            _logger = logger;
        }

        public async IAsyncEnumerable<CurrencyPair> StartAsync()
        {
            _isRunning = true;
            while(_isRunning)
            {
                foreach (var (symbol, pricing) in _currencyPairs)
                {
                    if (!_isRunning)
                    {
                        yield break;
                    }

                    var tick = NextTick();
                    var newPricing = pricing + tick;
                    _currencyPairs[symbol] = newPricing;

                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _logger.LogInformation($"[{timestamp}] Updated pricing for: {symbol}, {pricing:F} -> {newPricing:F} [{tick:F}]");
                    var currencyPair = new CurrencyPair(symbol, newPricing, timestamp);

                    PricingUpdated?.Invoke(this, currencyPair);

                    yield return currencyPair;

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        public Task StopAsync()
        {
            _isRunning = false;
            return Task.CompletedTask;
        }

        private decimal NextTick()
        {
            var sign = _random.Next(0, 2) == 0 ? -1 : 1; // Plus or minus randomness
            var tick = _random.NextDouble() / 20;
            
            return (decimal)(sign * tick);
        }

        public IEnumerable<string> GetSymbols()
        {
            return _currencyPairs.Keys.ToList();
        }
    }
}
