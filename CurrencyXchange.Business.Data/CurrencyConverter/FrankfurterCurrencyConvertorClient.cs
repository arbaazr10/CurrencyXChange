using CurrencyXChange.Domain.v1.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Text.Json;

namespace CurrencyXchange.Data.CurrencyConverter
{
    public class FrankfurterCurrencyConvertorClient : ICurrencyConvertorClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly FrankfurterApiOptions _frankfurterApiOptions;
        private readonly ILogger<FrankfurterCurrencyConvertorClient> _logger;

        public FrankfurterCurrencyConvertorClient(HttpClient httpClient, IMemoryCache cache, IOptions<FrankfurterApiOptions> frankfurterApiOptions, ILogger<FrankfurterCurrencyConvertorClient> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _frankfurterApiOptions = frankfurterApiOptions.Value;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            // Circuit breaker: Break after 5 failures, wait 30 seconds before allowing new requests
            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        public async Task<Currency> ConvertCurrencyAsync(string from, string to, decimal amount)
        {
            try
            {
                #pragma warning disable CS8603 // Possible null reference return.
                return await _circuitBreakerPolicy.ExecuteAsync(() =>
                    _retryPolicy.ExecuteAsync(async () =>
                    {
                        string cacheKey = $"CurrencyConvert_{from}_{to}_{amount}";

                        if (_cache.TryGetValue(cacheKey, out Currency? cachedRates))
                        {
                            return cachedRates;
                        }

                        var response = await _httpClient.GetAsync(
                            $"{_frankfurterApiOptions.FrankfurterBaseUrl}latest?from={from}&symbols={to}&amount={amount}");

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(content))
                            {
                                var currency = JsonSerializer.Deserialize<Currency>(
                                    content,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                                // Cache the deserialized object, not the response
                                _cache.Set(cacheKey, currency, TimeSpan.FromMinutes(5));

                                return currency!;
                            }
                        }

                        // Optional: handle failures explicitly
                        throw new Exception("Failed to fetch latest exchange rates.");
                    }));
                    #pragma warning restore CS8603 // Possible null reference return.
            }
            catch (BrokenCircuitException)
            {
                // Handle the case when the circuit is open
                throw new Exception("Service is temporarily unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"An error occurred while fetching latest rates: {ex.Message}");
            }
        }

        public async Task<Currency> FetchLatestRatesAsync(string baseCurrency)
        {
            try
            {
                #pragma warning disable CS8603 // Possible null reference return.
                return await _circuitBreakerPolicy.ExecuteAsync(() =>
                    _retryPolicy.ExecuteAsync(async () =>
                    {
                        string cacheKey = $"LatestRates_{baseCurrency}";

                        if (_cache.TryGetValue(cacheKey, out Currency? cachedRates))
                        {
                            return cachedRates;
                        }

                        _logger.LogInformation("Calling Frankfurter API: {Url}", $"{_frankfurterApiOptions.FrankfurterBaseUrl}latest?from={baseCurrency}");

                        var response = await _httpClient.GetAsync(
                            $"{_frankfurterApiOptions.FrankfurterBaseUrl}latest?from={baseCurrency}");

                        _logger.LogInformation($"Frankfurter API responded with {response.StatusCode}");

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            _logger.LogInformation($"Frankfurter API responded with {content}");

                            if (!string.IsNullOrEmpty(content))
                            {
                                var currency = JsonSerializer.Deserialize<Currency>(
                                    content,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                                // Cache the deserialized object, not the response
                                _cache.Set(cacheKey, currency, TimeSpan.FromMinutes(5));

                                return currency!;
                            }
                        }

                        // Optional: handle failures explicitly
                        throw new Exception("Failed to fetch latest exchange rates.");
                    }));
            #pragma warning restore CS8603 // Possible null reference return.
            }
            catch (BrokenCircuitException)
            {
                // Handle the case when the circuit is open
                throw new Exception("Service is temporarily unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"An error occurred while fetching latest rates: {ex.Message}");
            }
        }

        public async Task<CurrencyHistory> GetHistoricalRatesAsync(string baseCurrency, string startDate, string endDate)
        {

            try
            {
                #pragma warning disable CS8603 // Possible null reference return.
                return await _circuitBreakerPolicy.ExecuteAsync(() =>
                    _retryPolicy.ExecuteAsync(async () =>
                    {
                        string cacheKey = $"HistoryRates_{baseCurrency}_{startDate}_{endDate}";

                        if (_cache.TryGetValue(cacheKey, out CurrencyHistory? cachedRates))
                        {
                            return cachedRates;
                        }

                        var response = await _httpClient.GetAsync(
                            $"{_frankfurterApiOptions.FrankfurterBaseUrl}{startDate}..{endDate}?base={baseCurrency}");

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(content))
                            {
                                var currency = JsonSerializer.Deserialize<CurrencyHistory>(
                                    content,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                                // Cache the deserialized object, not the response
                                _cache.Set(cacheKey, currency, TimeSpan.FromMinutes(5));

                                return currency!;
                            }
                        }

                        // Optional: handle failures explicitly
                        
                        throw new Exception("Failed to fetch latest exchange rates.");
                    }));
                    #pragma warning restore CS8603 // Possible null reference return.
            }
            catch (BrokenCircuitException)
            {
                // Handle the case when the circuit is open
                throw new Exception("Service is temporarily unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"An error occurred while fetching latest rates: {ex.Message}");
            }

        }
    }
}

