namespace SkillSample.ExchangeRates.Backend.NBP
{
    public class NbpIntegrationConfiguration
    {
        internal const string HttpClientName = "NBP_INTEGRATOR";

        internal string ApiAddress => _apiAddress;
        internal string CurrencyTable => _currencyTable;

        public NbpIntegrationConfiguration()
        {
            _apiAddress = "http://api.nbp.pl";
            _currencyTable = "A";
        }

        public void UseApiAddress(string apiAddress) { _apiAddress = apiAddress; }
        public void UseCurrencyTable(string currencyTable) { _currencyTable = currencyTable; }

        private string _apiAddress;
        private string _currencyTable;
    }
}
