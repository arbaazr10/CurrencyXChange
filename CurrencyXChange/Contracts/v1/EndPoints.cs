namespace CurrencyXChange.Contracts.v1
{
    public class EndPoints
    {
        private const string Base = "";

        public static class CurrencyExchange
        {
            public const string Latest = Base + "latest";
            public const string Convert = Base + "convert";
            public const string Historical = Base + "historical";
        }
    }
}
