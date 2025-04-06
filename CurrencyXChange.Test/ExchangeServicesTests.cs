using CurrencyXchange.Business.Factory;
using CurrencyXchange.Business.Services.Exchange;
using CurrencyXChange.Domain.v1.Models;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyXchange.Data.CurrencyConverter;


namespace CurrencyXChange.Test
{
    public class ExchangeServicesTests
    {
        private readonly Mock<ICurrencyConvertorFactory> _mockFactory;
        private readonly Mock<ICurrencyConvertorClient> _mockConvertorClient;
        private readonly ExchangeServices _service;

        public ExchangeServicesTests()
        {
            _mockFactory = new Mock<ICurrencyConvertorFactory>();
            _mockConvertorClient = new Mock<ICurrencyConvertorClient>();

            _mockFactory.Setup(f => f.CreateConvertor(It.IsAny<string>()))
                        .Returns(_mockConvertorClient.Object);

            _service = new ExchangeServices(_mockFactory.Object);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldReturnConvertedCurrency()
        {
            // Arrange
            var expected = new Currency
            {
                amount = 100,
                @base = "USD",
                date = "2023-01-01",
                rates = new Dictionary<string, double> { { "EUR", 85.0 } }
            };

            _mockConvertorClient.Setup(c => c.ConvertCurrencyAsync("USD", "EUR", 100)).ReturnsAsync(expected);

            // Act
            var result = await _service.ConvertCurrencyAsync("USD", "EUR", 100, "frankfurter");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.amount, result.amount);
            Assert.Equal(expected.@base, result.@base);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldReturnLatestCurrency()
        {
            // Arrange
            var expected = new Currency
            {
                amount = 1,
                @base = "USD",
                date = "2023-01-01",
                rates = new Dictionary<string, double> { { "INR", 82.5 } }
            };

            _mockConvertorClient.Setup(c => c.FetchLatestRatesAsync("USD")).ReturnsAsync(expected);

            // Act
            var result = await _service.GetLatestRatesAsync("USD", "frankfurter");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD", result.@base);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldReturnPaginatedRates()
        {
            // Arrange
            var history = new CurrencyHistory
            {
                Amount = 1,
                BaseCurrency = "USD",
                StartDate = "2023-01-01",
                EndDate = "2023-01-05",
                Rates = new Dictionary<string, Dictionary<string, double>>
            {
                { "2023-01-01", new Dictionary<string, double> { { "EUR", 0.85 } } },
                { "2023-01-02", new Dictionary<string, double> { { "EUR", 0.86 } } },
                { "2023-01-03", new Dictionary<string, double> { { "EUR", 0.87 } } },
                { "2023-01-04", new Dictionary<string, double> { { "EUR", 0.88 } } },
                { "2023-01-05", new Dictionary<string, double> { { "EUR", 0.89 } } },
            }
            };

            _mockConvertorClient.Setup(c => c.GetHistoricalRatesAsync("USD", "2023-01-01", "2023-01-05"))
                          .ReturnsAsync(history);

            // Act
            var result = await _service.GetHistoricalRatesAsync("USD", "2023-01-01", "2023-01-05", page: 2, pageSize: 2, type: "frankfurter");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD", result.BaseCurrency);
            Assert.Equal(2, result.Rates.Count); // page 2 should have 2 records

        }
    }
}
