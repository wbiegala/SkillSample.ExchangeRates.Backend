using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillSample.ExchangeRates.Backend.Service.Mappings;
using SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate;

namespace SkillSample.ExchangeRates.Backend.Service.Controllers
{
    [Route("api/exchange-rate")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExchangeRateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRates([FromQuery] DateTime? date)
        {
            var result = await _mediator.Send(new GetDailyExchangeRateQuery { Date = date });

            if (result == null || result.EffectiveDate == null || result.TableNumber == null)
                return NotFound();

            return Ok(GetExchangeRatesMapper.Map(result));
        }
    }
}
