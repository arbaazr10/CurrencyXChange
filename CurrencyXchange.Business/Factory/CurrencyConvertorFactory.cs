using CurrencyXchange.Data.CurrencyConverter;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyXchange.Business.Factory
{
    public class CurrencyConvertorFactory:ICurrencyConvertorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyConvertorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICurrencyConvertorClient CreateConvertor(string type)
        {
            return type.ToLower() switch
            {
                "frankfurter" => _serviceProvider.GetRequiredService<FrankfurterCurrencyConvertorClient>(),
                _ => throw new ArgumentException("Invalid Client")
            };
        }
    }
}
