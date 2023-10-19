using MediatR;

namespace SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates
{
    public record DownloadExchangeRatesCommand : IRequest
    {
        public string Table { get; init; } = "A";
        public DateTime? Date { get; init; }
    }
}
