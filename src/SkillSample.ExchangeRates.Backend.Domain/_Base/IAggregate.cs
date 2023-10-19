namespace SkillSample.ExchangeRates.Backend.Domain.Base
{
    public interface IAggregate : IEntity
    {
        IReadOnlyList<IDomainEvent> Events { get; }
        void AddEvent(IDomainEvent @event);
    }
}
