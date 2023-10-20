using SkillSample.ExchangeRates.Backend.Contract;
using SkillSample.ExchangeRates.Backend.UseCases.Queries.GetDailyExchangeRate;

namespace SkillSample.ExchangeRates.Backend.Service.Mappings
{
    public static class GetExchangeRatesMapper
    {
        public static GetExchangeRatesResponseDto Map(GetDailyExchangeRateQueryResult data)
        {
            return new GetExchangeRatesResponseDto
            {
                EffectiveDate = data.EffectiveDate!.Value,
                TableNumber = data.TableNumber,
                CurrencyRates = data.Rates!.Select(r => new GetExchangeRatesResponseDto.CurrencyEntry
                {
                    CurrencyCode = r.CurrencyCode,
                    CurrencyName = r.CurrencyName,
                    Rate = r.Rate,
                })
            };
        }
    }
}
