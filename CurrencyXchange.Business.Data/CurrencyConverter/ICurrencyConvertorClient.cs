using CurrencyXChange.Domain.v1.Models;

namespace CurrencyXchange.Data.CurrencyConverter
{
    public interface ICurrencyConvertorClient
    {
        public Task<Currency> ConvertCurrencyAsync(string from, string to, decimal amount);
        public Task<Currency> FetchLatestRatesAsync(string baseCurrency);
        public Task<CurrencyHistory> GetHistoricalRatesAsync(string baseCurrency, string startDate, string endDate);
    }
}
