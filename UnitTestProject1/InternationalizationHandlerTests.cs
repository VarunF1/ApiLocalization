using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Api.Handler;

namespace UnitTestProject1
{
    public class InternationalizationHandlerTests
    {
        [Fact]
        public async Task ItShouldSetTheProvidedCulture()
        {
            // Arrange.
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/values");
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-IN"));

            var handler = new InternationalizationHandler()
            {
                InnerHandler = new TestDelegatingHandler()
            };

            // Act.
            var invoker = new HttpMessageInvoker(handler);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

            // Assert.
            Thread.CurrentThread.CurrentCulture.Name.Should().Be("en-IN");
            Thread.CurrentThread.CurrentCulture.NativeName.Should().Be("English (India)");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }

    public class TestDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK), cancellationToken);
        }
    }
}