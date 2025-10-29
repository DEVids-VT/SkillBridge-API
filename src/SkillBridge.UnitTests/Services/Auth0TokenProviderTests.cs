using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SkillBridge.Infrastructure.Configuration;
using SkillBridge.Infrastructure.Exceptions;
using SkillBridge.Services.Auth;
using Xunit;

namespace SkillBridge.UnitTests.Services
{
    public class Auth0TokenProviderTests
    {
        private class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;
            private int _callCount; // backing field
            public int CallCount => _callCount; // read-only property

            public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
            {
                _responder = responder ?? throw new ArgumentNullException(nameof(responder));
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _callCount);
                return await _responder(request, cancellationToken).ConfigureAwait(false);
            }
        }


        private static Auth0TokenProvider CreateProvider(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder, out TestHttpMessageHandler handler)
        {
            handler = new TestHttpMessageHandler(responder);
            var httpClient = new HttpClient(handler);

            var settings = new Auth0Settings
            {
                Domain = "example.auth0.com",
                ClientId = "cid",
                ClientSecret = "secret"
            };

            var options = Options.Create(settings);
            return new Auth0TokenProvider(httpClient, options);
        }

        [Fact]
        public async Task GetTokenAsync_ReturnsToken_WhenResponseIsSuccessful()
        {
            // Arrange
            var token = "access-xyz";
            var payload = new { access_token = token, expires_in = 3600, token_type = "Bearer" };
            var json = JsonConvert.SerializeObject(payload);

            var provider = CreateProvider((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            }, out _);

            // Act
            var result = await provider.GetTokenAsync();

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task GetTokenAsync_UsesCachedToken_WhenNotExpired()
        {
            // Arrange
            var token = "cached-token";
            var payload = new { access_token = token, expires_in = 3600, token_type = "Bearer" };
            var json = JsonConvert.SerializeObject(payload);

            var provider = CreateProvider((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            }, out var handler);

            // First call fetches token
            var first = await provider.GetTokenAsync();
            Assert.Equal(token, first);
            Assert.Equal(1, handler.CallCount);

            // Act: second call should use cache and not cause another HTTP call
            var second = await provider.GetTokenAsync();
            Assert.Equal(token, second);
            Assert.Equal(1, handler.CallCount);
        }

        [Fact]
        public async Task GetTokenAsync_RefreshesToken_WhenExpiresSoon()
        {
            // Arrange: expires_in small so token is considered expiring (provider uses a 5 minute safety window)
            var token1 = "short-lived-1";
            var token2 = "short-lived-2";

            var firstCalled = false;
            var provider = CreateProvider((req, ct) =>
            {
                if (!firstCalled)
                {
                    firstCalled = true;
                    var payload = new { access_token = token1, expires_in = 1, token_type = "Bearer" }; // expires almost immediately
                    var json = JsonConvert.SerializeObject(payload);
                    var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    return Task.FromResult(resp);
                }

                var payload2 = new { access_token = token2, expires_in = 3600, token_type = "Bearer" };
                var json2 = JsonConvert.SerializeObject(payload2);
                var resp2 = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json2, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp2);
            }, out var handler);

            // Act
            var first = await provider.GetTokenAsync();
            Assert.Equal(token1, first);
            Assert.Equal(1, handler.CallCount);

            // Immediate second call - because token expires soon (not > now+5min) provider should fetch again
            var second = await provider.GetTokenAsync();
            Assert.Equal(token2, second);
            Assert.Equal(2, handler.CallCount);
        }

        [Fact]
        public async Task GetTokenAsync_OnlyOneHttpRequest_WhenConcurrentCalls()
        {
            // Arrange
            var token = "concurrent-token";
            var payload = new { access_token = token, expires_in = 3600, token_type = "Bearer" };
            var json = JsonConvert.SerializeObject(payload);

            // Responder delays to amplify concurrency window
            var provider = CreateProvider(async (req, ct) =>
            {
                await Task.Delay(200, ct).ConfigureAwait(false);
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return resp;
            }, out var handler);

            // Act: fire multiple concurrent requests
            var tasks = Enumerable.Range(0, 5).Select(_ => provider.GetTokenAsync()).ToArray();
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.All(results, r => Assert.Equal(token, r));
            Assert.Equal(1, handler.CallCount);
        }

        [Fact]
        public async Task GetTokenAsync_ThrowsExternalServiceException_WhenPostFails()
        {
            // Arrange
            var provider = CreateProvider((req, ct) =>
            {
                throw new HttpRequestException("network");
            }, out _);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () => await provider.GetTokenAsync());
        }

        [Fact]
        public async Task GetTokenAsync_ThrowsExternalServiceException_WhenResponseNotSuccess()
        {
            // Arrange
            var provider = CreateProvider((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("error")
                };
                return Task.FromResult(resp);
            }, out _);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () => await provider.GetTokenAsync());
        }

        [Fact]
        public async Task GetTokenAsync_ThrowsExternalServiceException_WhenJsonInvalid()
        {
            // Arrange
            var provider = CreateProvider((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("this is not json", Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            }, out _);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () => await provider.GetTokenAsync());
        }

        [Fact]
        public async Task GetTokenAsync_ThrowsExternalServiceException_WhenTokenMissing()
        {
            // Arrange
            var payload = new { access_token = "", expires_in = 10, token_type = "Bearer" };
            var json = JsonConvert.SerializeObject(payload);

            var provider = CreateProvider((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(resp);
            }, out _);

            // Act & Assert
            await Assert.ThrowsAsync<ExternalServiceException>(async () => await provider.GetTokenAsync());
        }
    }
}
