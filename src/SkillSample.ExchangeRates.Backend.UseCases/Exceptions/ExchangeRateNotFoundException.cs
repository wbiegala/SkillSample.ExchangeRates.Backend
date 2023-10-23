namespace SkillSample.ExchangeRates.Backend.UseCases.Exceptions
{
    public class ExchangeRateNotFoundException : Exception
    {
        public ExchangeRateNotFoundException(DateTime date): base($"No data at {date.ToLongDateString}") { }
    }
}
