using SkillSample.ExchangeRates.Backend.Domain.Base;

namespace SkillSample.ExchangeRates.Backend.Domain.Model
{
    public class DailyRate : IEntity
    {
        public int Id { get; private set; }

        public int CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        public int ExchangeRateId { get; private set; }

        public decimal? Ask { get; private set; }
        public decimal? Bid { get; private set; }
        public decimal Mid { get; private set; }

        /// <summary>
        /// For EF
        /// </summary>
        protected DailyRate() { }

        internal DailyRate(Currency currency, decimal mid)
        {
            Currency = currency;
            Mid = mid;
        }

        internal DailyRate(Currency currency, decimal ask, decimal bid)
        {
            Currency = currency;
            Ask = ask;
            Bid = bid;
            Mid = (ask + bid) / 2;
        }
    }
}
