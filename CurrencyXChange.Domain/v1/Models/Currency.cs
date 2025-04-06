namespace CurrencyXChange.Domain.v1.Models
{

    //{"amount":1.0,"base":"EUR","date":"2025-04-04","rates":{"AUD":1.8098,"BGN":1.9558,"BRL":6.3407,"CAD":1.5696,"CHF":0.9407,"CNY":8.0518,"CZK":25.142,"DKK":7.4618,"GBP":0.84985,"HKD":8.5933,"HUF":405.7,"IDR":18751,"ILS":4.1244,"INR":94.43,"ISK":144.7,"JPY":160.56,"KRW":1601.11,"MXN":22.539,"MYR":4.906,"NOK":11.6815,"NZD":1.9613,"PHP":63.294,"PLN":4.2625,"RON":4.9773,"SEK":10.974,"SGD":1.4804,"THB":37.793,"TRY":42.03,"USD":1.1057,"ZAR":21.079}}
    public class Currency
    {
        public double? amount { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }
    public class Rates
    {
        public double? AUD { get; set; }
        public double? BGN { get; set; }
        public double? BRL { get; set; }
        public double? CAD { get; set; }
        public double? CHF { get; set; }
        public double? CNY { get; set; }
        public double? CZK { get; set; }
        public double? DKK { get; set; }
        public double? GBP { get; set; }
        public double? HKD { get; set; }
        public double? HUF { get; set; }
        public double? IDR { get; set; }
        public double? ILS { get; set; }
        public double? INR { get; set; }
        public double? ISK { get; set; }
        public double? JPY { get; set; }
        public double? KRW { get; set; }
        public double? MXN { get; set; }
        public double? MYR { get; set; }
        public double? NOK { get; set; }
        public double? NZD { get; set; }
        public double? PHP { get; set; }
        public double? PLN { get; set; }
        public double? RON { get; set; }
        public double? SEK { get; set; }
        public double? SGD { get; set; }
        public double? THB { get; set; }
        public double? TRY { get; set; }
        public double? USD { get; set; }
        public double? ZAR { get; set; }
    }



}
