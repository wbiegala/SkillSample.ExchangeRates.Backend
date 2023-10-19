using SkillSample.ExchangeRates.Backend.Domain.Base;

namespace SkillSample.ExchangeRates.Backend.Domain.Events
{
    public record ExchangeRatesUpdatedEvent : IDomainEvent
    {
        public Guid EventId { get; init; }
    }
}
