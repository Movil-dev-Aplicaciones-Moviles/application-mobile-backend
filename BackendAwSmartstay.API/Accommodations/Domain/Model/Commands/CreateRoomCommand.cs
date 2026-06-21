namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

/// <summary>
/// Command to create a new room within a specific hotel.
/// </summary>
/// <param name="HotelId">The unique identifier of the hotel where the room belongs.</param>
/// <param name="RoomTypeId">The unique identifier of the room type categorization.</param>
/// <param name="Price">The cost per night assigned to the room.</param>
/// <param name="Description">The detailed description of the physical room conditions.</param>
/// <param name="Amenities">The list of global system amenity descriptors configured for the room.</param>

public record CreateRoomCommand(
    int HotelId,  
    int RoomTypeId, 
    decimal Price, 
    string Description, 
    List<string> Amenities
);
