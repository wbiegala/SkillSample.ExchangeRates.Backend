namespace SkillSample.ExchangeRates.Backend.Domain.Base
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
    }
}
