using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.Domain.Model;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using SkillSample.ExchangeRates.Backend.NBP.ExchangeRates;
using SkillSample.ExchangeRates.Backend.UseCases.Exceptions;

namespace SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates
{
    internal class DownloadExchangeRatesCommandHandler : IRequestHandler<DownloadExchangeRatesCommand>
    {
        private readonly ExchangeRatesDbContext _dbContext;
        private readonly IExchangeRatesProvider _exchangeRatesProvider;
        private readonly ITimeService _timeService;
        private readonly IMediator _mediator;

        public DownloadExchangeRatesCommandHandler(
            ExchangeRatesDbContext dbContext,
            IExchangeRatesProvider exchangeRatesProvider,
            ITimeService timeService,
            IMediator mediator)
        {
            _dbContext = dbContext;
            _exchangeRatesProvider = exchangeRatesProvider;
            _timeService = timeService;
            _mediator = mediator;
        }

        public async Task Handle(DownloadExchangeRatesCommand request, CancellationToken cancellationToken)
        {
            if (request.Date.HasValue && request.Date.Value.Date > _timeService.Now.Date)
                throw new InvalidDateRangeException();

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var rates = request.Date.HasValue
                    ? await GetConcrateAsync(request.Date.Value.Date)
                    : await GetLatestAsync();

                if (rates == null)
                {
                    transaction.Rollback();
                    return;
                }

                var model = await PrepareExchangeForSaveAsync(rates);
                await _dbContext.ExchangeRates.AddAsync(model);
                await _dbContext.SaveChangesAsync();

                if (model.Events.Any())
                {
                    foreach (var @event in model.Events)
                    {
                        await _mediator.Publish(@event);
                    }
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Gets newest exchange rates from provider
        /// </summary>
        /// <returns>Newest data from provider or null when data is stored</returns>
        private async Task<ExchangeRatesDto?> GetLatestAsync()
        {
            var result = await _exchangeRatesProvider.GetExchangeRates();

            if (result == null)
                return null;

            var stored = await _dbContext.ExchangeRates
                .Where(er => er.EffectiveDate == result.EffectiveDate)
                .AnyAsync();

            return stored ? null : result;
        }

        /// <summary>
        /// Gets exchange rates from provider for given date
        /// </summary>
        /// <returns>Data from provider or null when data is stored</returns>
        private async Task<ExchangeRatesDto?> GetConcrateAsync(DateTime date)
        {
            var hasDataAlredy = await _dbContext.ExchangeRates
                .Where(er => er.EffectiveDate == date)
                .AnyAsync();

            if (hasDataAlredy)
                return null;

            return await _exchangeRatesProvider.GetExchangeRates(date);
        }

        /// <summary>
        /// Map and saves <see cref="ExchangeRate"/> in database
        /// </summary>
        private async Task<ExchangeRate> PrepareExchangeForSaveAsync(ExchangeRatesDto data)
        {
            var currencies = await MatchCurrenciesAsync(data.Rates);
            var rates = data.Rates.Select(r => new ExchangeRate.RateEntry
            {
                Currency = currencies.Single(c => c.Code == r.Code),
                Mid = r.Mid,
                Ask = r.Ask,
                Bid = r.Bid,
            });

            return new ExchangeRate(data.Table, data.No, data.TradingDate, data.EffectiveDate, rates);
        }

        /// <summary>
        /// Match existing collection of <see cref="Currency"/> to returned by <see cref="IExchangeRatesProvider"/>
        /// </summary>
        /// <returns>Merged set of stored and new currencies</returns>
        private async Task<IEnumerable<Currency>> MatchCurrenciesAsync(IEnumerable<ExchangeRatesDto.RateEntry> data)
        {
            var stored = await _dbContext.Currencies.ToListAsync();
            var newCurrencies = data.Where(r => !stored.Any(c => c.Code == r.Code))
                .Select(r => new Currency(r.Currency, r.Code));
            if (newCurrencies.Any())
            {
                stored.AddRange(newCurrencies);
            }

            return stored;
        }
    }
}
