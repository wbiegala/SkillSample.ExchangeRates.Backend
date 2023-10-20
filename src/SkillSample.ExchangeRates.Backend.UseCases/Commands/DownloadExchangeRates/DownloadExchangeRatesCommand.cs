using MediatR;

namespace SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates
{
    public record DownloadExchangeRatesCommand : IRequest
    {
        public DateTime? Date { get; init; }
    }
}
