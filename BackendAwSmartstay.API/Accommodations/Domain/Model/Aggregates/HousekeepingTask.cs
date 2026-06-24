using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;

/// <summary>
/// Represents a housekeeping task aggregate root.
/// Tracks the lifecycle of a cleaning task for a specific room.
/// Communicates with the Room aggregate exclusively through domain events.
/// No navigation properties to Room — only stores <see cref="RoomId"/> as a foreign key.
/// </summary>
public partial class HousekeepingTask : IEntityWithEvents
{
    private readonly List<IEvent> _domainEvents = new();

    /// <summary>
    /// Gets the read-only collection of domain events accumulated by this aggregate.
    /// Not mapped to the database.
    /// </summary>
    public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event on this aggregate.
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

    /// <summary>
    /// Initializes a new instance of the <see cref="HousekeepingTask"/> class.
    /// </summary>
    /// <param name="roomId">The identifier of the room to clean.</param>
    /// <param name="assignedHousekeeperId">The identifier of the assigned housekeeper.</param>
    /// <param name="taskType">The type of cleaning task.</param>
    public HousekeepingTask(int roomId, int assignedHousekeeperId, HousekeepingTaskType taskType)
    {
        RoomId = roomId;
        AssignedHousekeeperId = assignedHousekeeperId;
        TaskType = taskType;
        Status = HousekeepingTaskStatus.Pending;
    }

    // Private parameterless constructor for EF Core
    private HousekeepingTask()
    {
    }

    /// <summary>
    /// The unique identifier of the housekeeping task.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The identifier of the room to be cleaned.
    /// </summary>
    public int RoomId { get; private set; }

    /// <summary>
    /// The identifier of the assigned housekeeper (reference to IAM context).
    /// </summary>
    public int AssignedHousekeeperId { get; private set; }

    /// <summary>
    /// The current lifecycle status of the task.
    /// </summary>
    public HousekeepingTaskStatus Status { get; private set; }

    /// <summary>
    /// The type of cleaning task to perform.
    /// </summary>
    public HousekeepingTaskType TaskType { get; private set; }

    /// <summary>
    /// Starts the task, transitioning it to <see cref="HousekeepingTaskStatus.InProgress"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the task is not in Pending state.</exception>
    public void StartTask()
    {
        if (Status != HousekeepingTaskStatus.Pending)
            throw new InvalidOperationException("Only pending tasks can be started.");

        Status = HousekeepingTaskStatus.InProgress;
    }

    /// <summary>
    /// Completes the task, transitioning it to <see cref="HousekeepingTaskStatus.Completed"/>.
    /// Registers a <see cref="CleaningCompletedEvent"/> so that the Room aggregate
    /// can update its status to Clean via the event handler.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the task is not in InProgress state.</exception>
    public void CompleteTask()
    {
        if (Status != HousekeepingTaskStatus.InProgress)
            throw new InvalidOperationException("Only in-progress tasks can be completed.");

        Status = HousekeepingTaskStatus.Completed;

        AddDomainEvent(new CleaningCompletedEvent(RoomId));
    }
}
