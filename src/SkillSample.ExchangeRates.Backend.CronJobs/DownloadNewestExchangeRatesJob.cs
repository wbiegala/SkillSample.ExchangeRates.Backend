using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkillSample.ExchangeRates.Backend.Infrastructure.Time;
using SkillSample.ExchangeRates.Backend.UseCases.Commands.DownloadExchangeRates;

namespace SkillSample.ExchangeRates.Backend.CronJobs
{
    /// <summary>
    /// Job downloads exchange rates table from external API (NBP in that case) and save it into database
    /// </summary>
    public class DownloadNewestExchangeRatesJob
    {
        private readonly ILogger _logger;
        private readonly ITimeService _timeService;
        private readonly IMediator _mediator;

        public DownloadNewestExchangeRatesJob(
            ILoggerFactory loggerFactory,
            IMediator mediator,
            ITimeService timeService)
        {
            _mediator = mediator;
            _timeService = timeService;
            _logger = loggerFactory.CreateLogger<DownloadNewestExchangeRatesJob>();
        }

        [Function("DownloadNewestExchangeRatesJob")]
        public async Task Run([TimerTrigger("%DownloadNewestExchangeRatesJobCron%")] JobInfo myTimer)
        {
            var jobExecutionId = Guid.NewGuid();

            _logger.LogInformation($"[{jobExecutionId}] {nameof(DownloadNewestExchangeRatesJob)} started at {_timeService.Now}");

            try
            {
                await _mediator.Send(new DownloadExchangeRatesCommand());
                _logger.LogInformation($"[{jobExecutionId}] {nameof(DownloadNewestExchangeRatesJob)} finished processing at {_timeService.Now}");
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException)
                {
                    var httpError = ex as HttpRequestException;
                    if (httpError?.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"[{jobExecutionId}] External API throws error code 404: Not Found. There is no data for today");
                        return;
                    }
                }

                _logger.LogError(ex, $"[{jobExecutionId}] {nameof(DownloadNewestExchangeRatesJob)} failed.");                
            }
        }
    }
}
