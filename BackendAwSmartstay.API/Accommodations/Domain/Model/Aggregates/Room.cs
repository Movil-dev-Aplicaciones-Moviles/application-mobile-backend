using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Entities;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Events;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Shared.Domain.Model.Events;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;

/// <summary>
/// Represents a room aggregate in the accommodations domain.
/// </summary>
public partial class Room : IEntityWithEvents
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class with default values.
    /// </summary>
    public Room()
    {
        Description = string.Empty;
        Amenities = new List<string>();
        Status = RoomStatus.Clean;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Room"/> class from a create command.
    /// </summary>
    /// <param name="command">The command containing room creation data.</param>
    public Room(CreateRoomCommand command) : this()
    {
        RoomTypeId = command.RoomTypeId;
        HotelId = command.HotelId;
        Price = command.Price;
        Description = command.Description;
        Amenities = command.Amenities;
        Status = command.Status;

        // Register domain event — the Id will be assigned by EF Core on SaveChanges
        AddDomainEvent(new RoomCreatedEvent(Id, HotelId, RoomTypeId));
    }
    
    /// <summary>
    /// Updates the mutable information of the room aggregate.
    /// This method enforces business invariants during updates.
    /// </summary>
    /// <param name="roomTypeId">The new room type identifier.</param>
    /// <param name="price">The new price per night.</param>
    /// <param name="description">The new description.</param>
    /// <param name="amenities">The new list of amenities.</param>
    /// <param name="status">The new operational status of the room.</param>
    public void UpdateInformation(int roomTypeId, decimal price, string description, List<string> amenities, RoomStatus status)
    {
        // Validation logic can be placed here (e.g., Price > 0)
        if (price < 0) 
            throw new ArgumentException("Price cannot be negative.");

        var oldStatus = Status;

        RoomTypeId = roomTypeId;
        Price = price;
        Description = description;
        Amenities = amenities;
        Status = status;

        // Register domain event if the operational status changed
        if (oldStatus != status)
        {
            AddDomainEvent(new RoomStatusChangedEvent(Id, oldStatus, status));
        }
    }

    /// <summary>
    /// The unique identifier of the room.
    /// </summary>
    public int Id { get; }
    /// <summary>
    /// The identifier of the room type.
    /// </summary>
    public int RoomTypeId { get; private set; }
    
    /// <summary>
    /// The identifier of the hotel this room belongs to.
    /// </summary>
    public int HotelId { get; private set; }
    /// <summary>
    /// The price per night for the room.
    /// </summary>
    public decimal Price { get; private set; }
    
    /// <summary>
    /// The operational status of the room (Clean, Dirty, Maintenance, Reserved).
    /// Strongly typed via <see cref="RoomStatus"/> to eliminate primitive obsession.
    /// </summary>
    public RoomStatus Status { get; private set; }
    
    /// <summary>
    /// A description of the room.
    /// </summary>
    public string Description { get; private set; }
    /// <summary>
    /// The list of amenities provided by the room.
    /// </summary>
    public List<string> Amenities { get; private set; }

    // Navigation Properties
    /// <summary>
    /// The room type associated with this room.
    /// </summary>
    public virtual RoomType RoomType { get; private set; } 
    /// <summary>
    /// The hotel this room belongs to.
    /// </summary>
    public virtual Hotel Hotel { get; private set; }
}