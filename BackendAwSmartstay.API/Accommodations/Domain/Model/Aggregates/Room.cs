using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Entities;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;

namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;

/// <summary>
/// Represents a room aggregate in the accommodations domain.
/// </summary>
public partial class Room
{
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

        RoomTypeId = roomTypeId;
        Price = price;
        Description = description;
        Amenities = amenities;
        Status = status;
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