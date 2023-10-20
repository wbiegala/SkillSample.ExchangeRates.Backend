using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using System.Text.Json;

namespace SkillSample.ExchangeRates.Backend.NBP.ExchangeRates
{
    internal class ExchangeRatesProvider : IExchangeRatesProvider
    {
        private const string ExchangeRatesUrl = "api/exchangerates/tables";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITimeService _timeService;
        private readonly string _ratesTable;

        public ExchangeRatesProvider(
            IHttpClientFactory httpClientFactory,
            ITimeService timeService,
            NbpIntegrationConfiguration cfg)
        {
            _httpClientFactory = httpClientFactory;
            _timeService = timeService;
            _ratesTable = cfg.CurrencyTable;
        }

        public async Task<ExchangeRatesDto?> GetExchangeRates(DateTime? date = null)
        {
            using var client = _httpClientFactory.CreateClient(NbpIntegrationConfiguration.HttpClientName);

            var url = BuildUrl(_ratesTable, date);
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var serializedResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<ExchangeRatesDto>>(serializedResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); ;

            return result?.SingleOrDefault();
        }

        private static string BuildUrl(string table, DateTime? date)
        {
            var basic = $"{ExchangeRatesUrl}/{table}";

            return date.HasValue
                ? $"{basic}/{date.Value.ToString("yyyy-MM-dd")}"
                : basic;
        }
    }
}
