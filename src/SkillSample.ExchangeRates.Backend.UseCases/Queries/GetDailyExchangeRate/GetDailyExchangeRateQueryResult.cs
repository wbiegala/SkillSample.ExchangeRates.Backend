namespace SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate
{
    public record GetDailyExchangeRateQueryResult
    {
        public DateTime? EffectiveDate { get; init; }
        public string? TableNumber { get; init; }

        public IEnumerable<RateEntry>? Rates { get; init; }

        public record RateEntry
        {
            public string CurrencyName { get; init; }
            public string CurrencyCode { get; init; }
            public decimal Rate { get; init; }
        }
    }
}
