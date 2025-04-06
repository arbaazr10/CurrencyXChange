using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyXchange.Data.CurrencyConverter
{
    public class FrankfurterApiOptions
    {
        public string FrankfurterBaseUrl { get; set; } = string.Empty;
        public int CachingInMinutes { get; set; } = 5;
    }
}
