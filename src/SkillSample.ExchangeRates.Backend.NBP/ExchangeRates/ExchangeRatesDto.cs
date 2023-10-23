namespace SkillSample.ExchangeRates.Backend.NBP.ExchangeRates
{
    public class ExchangeRatesDto
    {
        public string Table {  get; set; }
        public string No {  get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? TradingDate { get; set; }
        public IEnumerable<RateEntry> Rates { get; set; }

        public class RateEntry
        {
            public string Currency {  get; set; }
            public string Code { get; set; }
            public decimal? Bid { get; set; }
            public decimal? Ask { get; set; }
            public decimal? Mid { get; set; }
        }
    }
}
