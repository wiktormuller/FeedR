using FeedR.Feeds.News.Messages;
using FeedR.Shared.Streaming;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;

namespace FeedR.Feeds.News.Tests.EndToEnd
{
    [ExcludeFromCodeCoverage]
    public class ApiTests
    {
        private readonly NewsTestApp _app;
        private readonly HttpClient _client;

        public ApiTests()
        {
            // Arrange per every test case
            _app = new NewsTestApp();
            _client = _app.CreateClient();
        }

        [Fact]
        public async Task get_base_endpoint_should_return_ok_status_code_and_service_name()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.ShouldBe("FeedR News Feed");
        }

        [Fact]
        public async Task post_news_should_return_accepted_status_code_and_publish_news_published_event()
        {
            // Arrange
            var tcs = new TaskCompletionSource<NewsPublished>();

            var streamSubscriber = _app.Services.GetRequiredService<IStreamSubscriber>();
            await streamSubscriber.SubscribeAsync<NewsPublished>("news", message =>
            {
                tcs.SetResult(message);
            });
            var request = new PublishNews("test news", "test category");
            
            // Act
            var response = await _client.PostAsJsonAsync("/news", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

            var @event = await tcs.Task; // Here we wait for the asynchronous processing of integration event to be processed
            @event.Title.ShouldBe("test news");
            @event.Category.ShouldBe("test category");
        }
    }
}
