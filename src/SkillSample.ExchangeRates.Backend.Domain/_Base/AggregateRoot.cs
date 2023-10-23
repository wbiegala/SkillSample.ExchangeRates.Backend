namespace SkillSample.ExchangeRates.Backend.Domain.Base
{
    public abstract class AggregateRoot : IAggregate
    {
        public int Id { get; private set; }
        public IReadOnlyList<IDomainEvent> Events => _events.ToList();

        public void AddEvent(IDomainEvent @event)
        {
            _events.Add(@event);
        }


        protected HashSet<IDomainEvent> _events = new();


    }
}
