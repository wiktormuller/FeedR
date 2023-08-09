using FeedR.Shared.Observability;
using Microsoft.AspNetCore.Http;

namespace FeedR.Shared.HTTP
{
    internal class CorrelationIdMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CorrelationIdMessageHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = _contextAccessor.HttpContext?.GetCorrelationId() ?? Guid.NewGuid().ToString("N");
            request.Headers.AddCorrelationId(correlationId);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
