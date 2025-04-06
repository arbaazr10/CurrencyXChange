using CurrencyXchange.Data.CurrencyConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyXchange.Business.Factory
{
    public interface ICurrencyConvertorFactory
    {
        public ICurrencyConvertorClient CreateConvertor(string type);
    }
}
