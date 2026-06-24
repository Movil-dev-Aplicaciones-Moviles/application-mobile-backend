using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;

/// <summary>
/// Represents a maintenance request aggregate root.
/// Tracks the lifecycle of a repair request for a specific room.
/// Communicates with the Room aggregate exclusively through domain events.
/// No navigation properties to Room — only stores <see cref="RoomId"/> as a foreign key.
/// </summary>
public partial class MaintenanceRequest : IEntityWithEvents
{
    private readonly List<IEvent> _domainEvents = new();

    /// <inheritdoc />
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event on this aggregate.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="MaintenanceRequest"/> class.
    /// </summary>
    /// <param name="roomId">The identifier of the room requiring maintenance.</param>
    /// <param name="description">A description of the issue.</param>
    /// <param name="priority">The priority level of the request.</param>
    public MaintenanceRequest(int roomId, string description, MaintenancePriority priority)
    {
        RoomId = roomId;
        Description = description;
        Priority = priority;
        Status = MaintenanceStatus.Pending;
    }

    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private MaintenanceRequest()
    {
        Description = string.Empty;
    }

    /// <summary>
    /// The unique identifier of the maintenance request.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The identifier of the room requiring maintenance.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    /// A description of the issue.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// The identifier of the assigned technician (reference to IAM context).
    /// Nullable until a technician is assigned.
    /// </summary>
    public int? AssignedTechnicianId { get; private set; }

    /// <summary>
    /// The current lifecycle status of the request.
    /// </summary>
    public MaintenanceStatus Status { get; private set; }

    /// <summary>
    /// The priority level of the request.
    /// </summary>
    public MaintenancePriority Priority { get; private set; }

    /// <summary>
    /// Starts the maintenance work, transitioning to <see cref="MaintenanceStatus.InProgress"/>
    /// and registering a <see cref="MaintenanceStartedEvent"/> so the Room aggregate
    /// can update its status to <see cref="RoomStatus.Maintenance"/>.
    /// </summary>
    /// <param name="technicianId">The identifier of the assigned technician.</param>
    /// <exception cref="InvalidOperationException">Thrown if the request is not in Pending state.</exception>
    public void StartMaintenance(int technicianId)
    {
        if (Status != MaintenanceStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be started.");

        AssignedTechnicianId = technicianId;
        Status = MaintenanceStatus.InProgress;

        AddDomainEvent(new MaintenanceStartedEvent(RoomId));
    }

    /// <summary>
    /// Resolves the maintenance work, transitioning to <see cref="MaintenanceStatus.Resolved"/>
    /// and registering a <see cref="MaintenanceResolvedEvent"/> so the Room aggregate
    /// can update its status to <see cref="RoomStatus.Dirty"/> (requiring cleaning).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the request is not in InProgress state.</exception>
    public void ResolveMaintenance()
    {
        if (Status != MaintenanceStatus.InProgress)
            throw new InvalidOperationException("Only in-progress requests can be resolved.");

        Status = MaintenanceStatus.Resolved;

        AddDomainEvent(new MaintenanceResolvedEvent(RoomId));
    }
}
