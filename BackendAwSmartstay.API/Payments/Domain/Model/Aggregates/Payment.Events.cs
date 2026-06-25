using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Payments.Domain.Model.Aggregates;

/// <summary>
/// Partial class for Payment that adds domain event support.
/// </summary>
public partial class Payment : IEntityWithEvents
{
    private readonly List<IEvent> _domainEvents = new();

    /// <summary>
    /// Gets the read-only collection of domain events accumulated by this aggregate.
    /// Not mapped to the database.
    /// </summary>
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event on this aggregate.
    /// Events are cleared after being dispatched by the UnitOfWork.
    /// </summary>
    /// <param name="domainEvent">The domain event to register.</param>
    public void AddDomainEvent(IEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all registered domain events after they have been dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
