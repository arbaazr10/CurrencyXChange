using CurrencyXChange.Domain.v1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyXchange.Business.Services.Exchange
{
    public interface IExchangeServices
    {
        Task<Currency> ConvertCurrencyAsync(string from, string to, decimal amount, string type);
        Task<CurrencyHistory> GetHistoricalRatesAsync(string baseCurrency, string startDate, string endDate, int page, int pageSize, string type);
        Task<Currency> GetLatestRatesAsync(string baseCurrency,string type);
    }
}
