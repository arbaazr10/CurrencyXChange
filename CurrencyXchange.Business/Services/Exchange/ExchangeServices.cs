using CurrencyXchange.Business.Factory;
using CurrencyXChange.Domain.v1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyXchange.Business.Services.Exchange
{
    public class ExchangeServices : IExchangeServices
    {
        ICurrencyConvertorFactory _currencyConvertorFactory;

        public ExchangeServices(ICurrencyConvertorFactory currencyConvertorFactory)
        {
            _currencyConvertorFactory = currencyConvertorFactory;
        }

        public async Task<Currency> ConvertCurrencyAsync(string from, string to, decimal amount,string type)
        {
            var convertorClient = _currencyConvertorFactory.CreateConvertor(type);
            return await convertorClient.ConvertCurrencyAsync(from,to,amount);
        }


        public async Task<Currency> GetLatestRatesAsync(string baseCurrency,string type)
        {
            var convertorClient = _currencyConvertorFactory.CreateConvertor(type);
            return await convertorClient.FetchLatestRatesAsync(baseCurrency);
        }

        public async Task<CurrencyHistory>  GetHistoricalRatesAsync(string baseCurrency, string startDate, string endDate, int page, int pageSize, string type)
        {
            var convertorClient = _currencyConvertorFactory.CreateConvertor(type);


            var fullHistory = await convertorClient.GetHistoricalRatesAsync(baseCurrency, startDate, endDate);

            // Order rates by date 
            var orderedRates = fullHistory.Rates.OrderBy(r => r.Key); 

            // Apply pagination
            var pagedRates = orderedRates
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToDictionary(r => r.Key, r => r.Value);

            // Return a new CurrencyHistory object with paginated rates
            return new CurrencyHistory
            {
                Amount = fullHistory.Amount,
                BaseCurrency = fullHistory.BaseCurrency,
                StartDate = fullHistory.StartDate,
                EndDate = fullHistory.EndDate,
                Rates = pagedRates
            };
        }
    }
}
