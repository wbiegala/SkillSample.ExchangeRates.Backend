using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.Domain.Base;
using SkillSample.ExchangeRates.Backend.Domain.Events;
using SkillSample.ExchangeRates.Backend.Domain.Model;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using SkillSample.ExchangeRates.Backend.NBP.ExchangeRates;
using SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates;
using SkillSample.ExchangeRates.Backend.UseCases.Exceptions;
using System.Text.Json;

namespace SkillSample.ExchangeRates.Backend.UseCases.UnitTests.Commands
{
    [TestFixture]
    public class DownloadExchangeRatesCommandHandlerTests
    {
        [Test(Description = "Most often scenario when we download newest data")]
        public async Task Handle_NoDateInRequest_SavesNewestData()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            _providerMock.Setup(s => s.GetExchangeRates(It.IsAny<DateTime?>()))
                .ReturnsAsync(Deserialize(NEXT_DAY_DTO));
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT
            var request = new DownloadExchangeRatesCommand();
            await handler.Handle(request, CancellationToken.None);

            // ASSERT
            _providerMock.Verify(s => s.GetExchangeRates(It.Is<DateTime?>(val => val == null)), Times.Once);
            _mediatorMock.Verify(s => s.Publish(It.Is<IDomainEvent>(@event => @event is ExchangeRatesUpdatedEvent), 
                It.IsAny<CancellationToken>()), Times.Once);

            var saved = _dbContext.ExchangeRates
                .Include(er => er.Rates).ThenInclude(r => r.Currency)
                .Where(er => er.TableNumber == "204/A/NBP/2023").SingleOrDefault();
            Assert.IsNotNull(saved);
            Assert.That(saved.Rates.Count, Is.EqualTo(4));
        }

        [Test(Description = "Often scenerios when we download newest data but already have data for this day. " +
            "Possible, because data emitter (NBP in this case) can skips weekends and holidays.")]
        public async Task Handle_NoDateInRequestButNewestDataAlreadyStored_SkipsSave()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            _providerMock.Setup(s => s.GetExchangeRates(It.IsAny<DateTime?>()))
                .ReturnsAsync(Deserialize(ALREADY_STORED_DAY_DTO));
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT
            var request = new DownloadExchangeRatesCommand();
            await handler.Handle(request, CancellationToken.None);

            // ASSERT
            _providerMock.Verify(s => s.GetExchangeRates(It.Is<DateTime?>(val => val == null)), Times.Once);
            _mediatorMock.Verify(s => s.Publish(It.IsAny<IDomainEvent>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ConcrateDate_SavesConcrateData()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            _providerMock.Setup(s => s.GetExchangeRates(It.IsAny<DateTime?>()))
                .ReturnsAsync(Deserialize(PAST_DAY_DTO));
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT
            var request = new DownloadExchangeRatesCommand { Date = new DateTime(2023, 10, 5) };
            await handler.Handle(request, CancellationToken.None);

            // ASSERT
            _providerMock.Verify(s => s.GetExchangeRates(It.Is<DateTime?>(val => val == new DateTime(2023, 10, 5))), Times.Once);
            _mediatorMock.Verify(s => s.Publish(It.Is<IDomainEvent>(@event => @event is ExchangeRatesUpdatedEvent),
                It.IsAny<CancellationToken>()), Times.Once);

            var saved = _dbContext.ExchangeRates
                .Include(er => er.Rates).ThenInclude(r => r.Currency)
                .Where(er => er.TableNumber == "198/A/NBP/2023").SingleOrDefault();
            Assert.IsNotNull(saved);
            Assert.That(saved.Rates.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Handle_ConcrateDateButDataAlreadyStored_SkipsSaveAndSkipProviderCall()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT
            var request = new DownloadExchangeRatesCommand { Date = new DateTime(2023, 10, 18) };
            await handler.Handle(request, CancellationToken.None);

            // ASSERT
            _providerMock.Verify(s => s.GetExchangeRates(It.IsAny<DateTime?>()), Times.Never);
            _mediatorMock.Verify(s => s.Publish(It.IsAny<IDomainEvent>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Handle_ConcrateFutureDate_ThrowsException()
        {
            // ARRANGE
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT & ARRANGE
            var request = new DownloadExchangeRatesCommand { Date = new DateTime(2024, 10, 18) };
            Assert.ThrowsAsync<InvalidDateRangeException>(async () => await handler.Handle(request, CancellationToken.None));
        }

        [Test]
        public void Handle_ProviderThrowsHttpException_ThrowsException()
        {
            // ARRANGE 
            _timeServiceMock.Setup(s => s.Now).Returns(NOW);
            _providerMock.Setup(s => s.GetExchangeRates(It.IsAny<DateTime?>()))
                .ThrowsAsync(new HttpRequestException());
            var handler = new DownloadExchangeRatesCommandHandler(
                _dbContext,
                _providerMock.Object,
                _timeServiceMock.Object,
                _mediatorMock.Object);

            // ACT & ASSERT
            var request = new DownloadExchangeRatesCommand();
            Assert.ThrowsAsync<HttpRequestException>(async () => await handler.Handle(request, CancellationToken.None));
        }


        [SetUp]
        public void Setup()
        {
            _providerMock.Reset();
            _mediatorMock.Reset();

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

        private Mock<IExchangeRatesProvider> _providerMock = new();
        private Mock<ITimeService> _timeServiceMock = new();
        private Mock<IMediator> _mediatorMock = new();
        private ExchangeRatesDbContext _dbContext;

        private const string TABLE_1 = "A";
        private const string TABLE_NUMBER_1 = "202/A/NBP/2023";
        private readonly DateTime EFFECTIVE_DATE_1 = new DateTime(2023, 10, 17);

        private const string TABLE_2 = "A";
        private const string TABLE_NUMBER_2 = "203/A/NBP/2023";
        private readonly DateTime EFFECTIVE_DATE_2 = new DateTime(2023, 10, 18);

        private readonly DateTime NOW = new DateTime(2023, 10, 20, 12, 15, 55);

        private static ExchangeRatesDto? Deserialize(string json) =>
            JsonSerializer.Deserialize<ExchangeRatesDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        private const string NEXT_DAY_DTO = @"
{
    ""table"": ""A"",
    ""no"": ""204/A/NBP/2023"",
    ""effectiveDate"": ""2023-10-19"",
    ""rates"": [
        {
            ""currency"": ""bat (Tajlandia)"",
            ""code"": ""THB"",
            ""mid"": 0.1155
        },
        {
                ""currency"": ""dolar amerykański"",
                ""code"": ""USD"",
                ""mid"": 4.2194
        },
        {
                ""currency"": ""dolar australijski"",
                ""code"": ""AUD"",
                ""mid"": 2.6637
        },
        {
            ""currency"": ""euro"",
            ""code"": ""EUR"",
            ""mid"": 4.4675
        }
	]
}";

        private const string ALREADY_STORED_DAY_DTO = @"
{
    ""table"": ""A"",
    ""no"": ""204/A/NBP/2023"",
    ""effectiveDate"": ""2023-10-18"",
    ""rates"": [
        {
            ""currency"": ""bat (Tajlandia)"",
            ""code"": ""THB"",
            ""mid"": 0.1155
        },
        {
                ""currency"": ""dolar amerykański"",
                ""code"": ""USD"",
                ""mid"": 4.2194
        },
        {
                ""currency"": ""dolar australijski"",
                ""code"": ""AUD"",
                ""mid"": 2.6637
        },
        {
            ""currency"": ""euro"",
            ""code"": ""EUR"",
            ""mid"": 4.4675
        }
	]
}";

        private const string PAST_DAY_DTO = @"
{
    ""table"": ""A"",
    ""no"": ""198/A/NBP/2023"",
    ""effectiveDate"": ""2023-10-05"",
    ""rates"": [
        {
            ""currency"": ""bat (Tajlandia)"",
            ""code"": ""THB"",
            ""mid"": 0.1155
        },
        {
                ""currency"": ""dolar amerykański"",
                ""code"": ""USD"",
                ""mid"": 4.2194
        },
        {
            ""currency"": ""euro"",
            ""code"": ""EUR"",
            ""mid"": 4.4675
        }
	]
}";
    }
}
