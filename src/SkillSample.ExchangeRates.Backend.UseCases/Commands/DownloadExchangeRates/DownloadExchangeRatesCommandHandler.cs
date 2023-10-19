using MediatR;
using SkillSample.ExchangeRates.Backend.Data;

namespace SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates
{
    internal class DownloadExchangeRatesCommandHandler : IRequestHandler<DownloadExchangeRatesCommand>
    {
        private readonly ExchangeRatesDbContext _dbContext;

        public DownloadExchangeRatesCommandHandler(ExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DownloadExchangeRatesCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
