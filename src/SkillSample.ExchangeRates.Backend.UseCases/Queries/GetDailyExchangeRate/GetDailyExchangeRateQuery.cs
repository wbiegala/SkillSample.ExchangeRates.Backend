using MediatR;

namespace SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate
{
    public record GetDailyExchangeRateQuery : IRequest<GetDailyExchangeRateQueryResult>
    {
        public DateTime? Date { get; init; }
    }
}
