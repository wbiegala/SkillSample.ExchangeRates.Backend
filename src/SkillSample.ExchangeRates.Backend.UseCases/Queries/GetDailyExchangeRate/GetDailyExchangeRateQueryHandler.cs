using MediatR;
using Microsoft.EntityFrameworkCore;
using SkillSample.ExchangeRates.Backend.Data;
using SkillSample.ExchangeRates.Backend.Domain.Model;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using SkillSample.ExchangeRates.Backend.UseCases.Exceptions;

namespace SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate
{
    internal class GetDailyExchangeRateQueryHandler : IRequestHandler<GetDailyExchangeRateQuery, GetDailyExchangeRateQueryResult>
    {
        private readonly ExchangeRatesDbContext _dbContext;
        private readonly ITimeService _timeService;

        public GetDailyExchangeRateQueryHandler(ExchangeRatesDbContext dbContext, ITimeService timeService)
        {
            _dbContext = dbContext;
            _timeService = timeService;
        }

        public async Task<GetDailyExchangeRateQueryResult> Handle(GetDailyExchangeRateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Date > _timeService.Now.Date)
                    throw new InvalidDateRangeException();

                var result = await ApplyRequestFiltering(_dbContext.ExchangeRates, request)
                    .Include(e => e.Rates)
                        .ThenInclude(r => r.Currency)
                    .SingleOrDefaultAsync();

                if (result == null)
                    throw new ExchangeRateNotFoundException(request.Date ?? _timeService.Now.Date);

                return new GetDailyExchangeRateQueryResult
                {
                    EffectiveDate = result?.EffectiveDate,
                    TableNumber = result?.TableNumber,
                    Rates = result?.Rates.Select(rate => new GetDailyExchangeRateQueryResult.RateEntry
                    {
                        CurrencyName = rate.Currency.Name,
                        CurrencyCode = rate.Currency.Code,
                        Rate = rate.Mid,
                    }),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private IQueryable<ExchangeRate> ApplyRequestFiltering(IQueryable<ExchangeRate> query, GetDailyExchangeRateQuery request)
        {
            if (request.Date == null)
            {
                return query.OrderByDescending(e => e.EffectiveDate)
                    .Take(1);
            }

            return query.Where(e => e.EffectiveDate == request.Date);
        }
    }
}
