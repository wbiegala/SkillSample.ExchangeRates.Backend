using SkillSample.ExchangeRates.Backend.Domain.Events;
using SkillSample.ExchangeRates.Backend.Domain.Model;

namespace SkillSample.ExchangeRates.Backend.Domain.UnitTests.Model
{
    [TestFixture]
    public class ExchangeRateTests
    {
        [Test]
        public void Constructor_WhenRatesAreWithAskAndBid_EventEmitted()
        {
            // ARRANGE
            var entries = new List<ExchangeRate.RateEntry>
            {
                new ExchangeRate.RateEntry { Currency = USD, Ask = 4.2522m, Bid = 4.1680m },
                new ExchangeRate.RateEntry { Currency = EUR, Ask = 4.4865m, Bid = 4.3977m }
            };

            // ACT
            var exchange = new ExchangeRate(TABLE, TABLE_NUMBER, TRADING_DATE, EFFECTIVE_DATE, entries);

            // ASSERT
            Assert.That(exchange, Is.Not.Null);
            Assert.That(exchange.Rates.Count, Is.EqualTo(entries.Count));
            Assert.That(exchange.Events.Single() is ExchangeRatesUpdatedEvent);
        }

        [Test]
        public void Constructor_WhenRatesHasOnlyMid_EventEmitted()
        {
            // ARRANGE
            var entries = new List<ExchangeRate.RateEntry>
            {
                new ExchangeRate.RateEntry { Currency = USD, Mid = 4.2264m },
                new ExchangeRate.RateEntry { Currency = EUR, Mid = 4.4529m }
            };

            // ACT
            var exchange = new ExchangeRate(TABLE, TABLE_NUMBER, TRADING_DATE, EFFECTIVE_DATE, entries);

            // ASSERT
            Assert.That(exchange, Is.Not.Null);
            Assert.That(exchange.Rates.Count, Is.EqualTo(entries.Count));
            Assert.That(exchange.Events.Single() is ExchangeRatesUpdatedEvent);
        }

        private const string TABLE = "A";
        private const string TABLE_NUMBER = "102/RUDY";
        private readonly DateTime TRADING_DATE = new DateTime(2023, 10, 19);
        private readonly DateTime EFFECTIVE_DATE = new DateTime(2023, 10, 18);
        private Currency USD => new Currency("Dolar amerykański", "USD");
        private Currency EUR => new Currency("Euro", "EUR");
    }
}
