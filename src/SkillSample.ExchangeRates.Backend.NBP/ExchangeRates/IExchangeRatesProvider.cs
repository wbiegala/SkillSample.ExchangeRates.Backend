namespace SkillSample.ExchangeRates.Backend.NBP.ExchangeRates
{
    public interface IExchangeRatesProvider
    {
        Task<ExchangeRatesDto?> GetExchangeRates(DateTime? date = null);
    }
}
