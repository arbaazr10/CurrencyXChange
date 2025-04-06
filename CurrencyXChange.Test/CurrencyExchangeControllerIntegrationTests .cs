using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using CurrencyXChange;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CurrencyXChange.Test
{
    public class CurrencyExchangeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CurrencyExchangeControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("USD")]
        public async Task GetLatestRates_ShouldReturnOk(string baseCurrency)
        {
            // Arrange
            var url = $"/api/v1/CurrenctExchange/latest?baseCurrency={baseCurrency}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<object>();
            result.Should().NotBeNull();
        }


    }
}