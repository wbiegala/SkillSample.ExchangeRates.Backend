using MediatR;

namespace SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate
{
    public record GetDailyExchangeRateQuery : IRequest<GetDailyExchangeRateQueryResult>
    {
        /// <summary>
        /// Date for exchange rates. If null query will return latest results
        /// </summary>
        public DateTime? Date { get; init; }
    }
}
