using MediatR;

namespace SkillSample.ExchangeRates.Backend.Domain.Base
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
    }
}
