using SkillSample.ExchangeRates.Backend.Domain.Base;
using SkillSample.ExchangeRates.Backend.Domain.Events;

namespace SkillSample.ExchangeRates.Backend.Domain.Model
{
    public class ExchangeRate : AggregateRoot
    {
        public string Table {  get; private set; }
        public string TableNumber { get; private set; }
        public DateTime TradingDate { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public IReadOnlyCollection<DailyRate> Rates => _rates.ToList();

        /// <summary>
        /// Used by EF
        /// </summary>
        protected ExchangeRate() { }

        public ExchangeRate(string table, string tableNumber, DateTime tradingDate, DateTime effectiveDate, IEnumerable<RateEntry> rates)
        {
            Table = table;
            TableNumber = tableNumber;
            TradingDate = tradingDate;
            EffectiveDate = effectiveDate;

            foreach (var rate in rates)
            {
                if (rate.Mid.HasValue)
                {
                    _rates.Add(new DailyRate(rate.Currency, rate.Mid!.Value));                    
                }
                else
                {
                    _rates.Add(new DailyRate(rate.Currency, rate.Ask!.Value, rate.Bid!.Value));
                }
            }

            AddEvent(new ExchangeRatesUpdatedEvent());
        }

        public record RateEntry
        {
            public Currency Currency { get; init; }
            public decimal? Ask { get; init; }
            public decimal? Bid { get; init; }
            public decimal? Mid { get; init; }
        }

        private HashSet<DailyRate> _rates = new();
    }
}
