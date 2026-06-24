namespace BackendAwSmartstay.API.Shared.Domain.Model.Events;

/// <summary>
/// Defines an entity that can accumulate domain events during its lifecycle.
/// The UnitOfWork checks for this interface to dispatch events before/after persisting.
/// </summary>
public interface IEntityWithEvents
{
    /// <summary>
    /// Gets the read-only collection of domain events accumulated by this entity.
    /// </summary>
    IReadOnlyCollection<IEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all registered domain events after they have been dispatched.
    /// </summary>
    void ClearDomainEvents();
}
