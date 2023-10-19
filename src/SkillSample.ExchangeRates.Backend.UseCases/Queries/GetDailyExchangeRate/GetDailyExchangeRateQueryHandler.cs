using MediatR;
using SkillSample.ExchangeRates.Backend.Data;

namespace SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate
{
    internal class GetDailyExchangeRateQueryHandler : IRequestHandler<GetDailyExchangeRateQuery, GetDailyExchangeRateQueryResult>
    {
        private readonly ExchangeRatesDbContext _dbContext;

        public GetDailyExchangeRateQueryHandler(ExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetDailyExchangeRateQueryResult> Handle(GetDailyExchangeRateQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
