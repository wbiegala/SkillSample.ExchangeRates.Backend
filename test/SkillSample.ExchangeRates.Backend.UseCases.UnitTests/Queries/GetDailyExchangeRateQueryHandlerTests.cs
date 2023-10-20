using Moq;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.Domain.Model;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using SkillSample.ExchangeRates.Backend.UseCases.Exceptions;
using SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate;

namespace SkillSample.ExchangeRates.Backend.UseCases.UnitTests.Queries
{
    [TestFixture]
    public class GetDailyExchangeRateQueryHandlerTests
    {
        [Test]
        public async Task Handle_ForNullDateInRequest_ReturnLatestRates()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new GetDailyExchangeRateQueryHandler(_dbContext, _timeServiceMock.Object);
            var request = new GetDailyExchangeRateQuery();

            // ACT
            var result = await handler.Handle(request, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Date, Is.EqualTo(EFFECTIVE_DATE_2));
            Assert.That(result.TableNumber, Is.EqualTo(TABLE_NUMBER_2));
        }

        [Test]
        public async Task Handle_ForConcrateDateInRequest_ReturnRatesFromGivenDay()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new GetDailyExchangeRateQueryHandler(_dbContext, _timeServiceMock.Object);
            var request = new GetDailyExchangeRateQuery { Date = EFFECTIVE_DATE_1 };

            // ACT
            var result = await handler.Handle(request, CancellationToken.None);

            // ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Date, Is.EqualTo(EFFECTIVE_DATE_1));
            Assert.That(result.TableNumber, Is.EqualTo(TABLE_NUMBER_1));
        }

        [Test]
        public void Handle_ForConcrateDateInRequestWhenThereIsNoData_ThrowsException()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new GetDailyExchangeRateQueryHandler(_dbContext, _timeServiceMock.Object);
            var request = new GetDailyExchangeRateQuery { Date = new DateTime(2019, 12, 31) };

            // ACT & ASSERT
            Assert.ThrowsAsync<ExchangeRateNotFoundException>(async () => 
                await handler.Handle(request, CancellationToken.None));
        }

        [Test]
        public void Handle_ForFutureDateInRequest_ThrowsException()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new GetDailyExchangeRateQueryHandler(_dbContext, _timeServiceMock.Object);
            var request = new GetDailyExchangeRateQuery { Date = new DateTime(2024, 1, 1) };

            // ACT & ASSERT
            Assert.ThrowsAsync<InvalidDateRangeException>(async () =>
                await handler.Handle(request, CancellationToken.None));
        }

        [SetUp]
        public void Setup()
        {
            _dbContext = DatabaseMockHelper.GetInMemoryDbContext();

            var USD = new Currency("Dolar amerykański", "USD");
            var EUR = new Currency("Euro", "EUR");

            _dbContext.Currencies.Add(USD);
            _dbContext.Currencies.Add(EUR);

            _dbContext.SaveChanges();

            var rate1 = new ExchangeRate(TABLE_1, TABLE_NUMBER_1, null, EFFECTIVE_DATE_1, new List<ExchangeRate.RateEntry>
            {
                new ExchangeRate.RateEntry { Currency = USD, Mid = 4.2264m },
                new ExchangeRate.RateEntry { Currency = EUR, Mid = 4.4529m }
            });

            var rate2 = new ExchangeRate(TABLE_2, TABLE_NUMBER_2, null, EFFECTIVE_DATE_2, new List<ExchangeRate.RateEntry>
            {
                new ExchangeRate.RateEntry { Currency = USD, Mid = 4.2252m },
                new ExchangeRate.RateEntry { Currency = EUR, Mid = 4.4566m }
            });

            _dbContext.ExchangeRates.Add(rate1);
            _dbContext.ExchangeRates.Add(rate2);
            _dbContext.SaveChanges();
        }

        private Mock<ITimeService> _timeServiceMock = new();
        private ExchangeRatesDbContext _dbContext;

        private const string TABLE_1 = "A";
        private const string TABLE_NUMBER_1 = "101/DALMATYŃCZYKÓW";
        private readonly DateTime EFFECTIVE_DATE_1 = new DateTime(2023, 10, 17);

        private const string TABLE_2 = "A";
        private const string TABLE_NUMBER_2 = "102/RUDY";
        private readonly DateTime EFFECTIVE_DATE_2 = new DateTime(2023, 10, 18);

        private readonly DateTime NOW = new DateTime(2023, 10, 20, 12, 15, 55);
    }
}
