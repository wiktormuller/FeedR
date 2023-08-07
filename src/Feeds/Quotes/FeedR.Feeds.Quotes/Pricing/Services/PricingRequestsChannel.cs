using FeedR.Feeds.Quotes.Requests;
using System.Threading.Channels;

namespace FeedR.Feeds.Quotes.Pricing.Services
{
    // We need it, because there's no direct way to communicate from the WebApi process to BackgroundService process
    internal sealed class PricingRequestsChannel
    {
        public readonly Channel<IPricingRequest> Requests = Channel.CreateUnbounded<IPricingRequest>();
    }
}
