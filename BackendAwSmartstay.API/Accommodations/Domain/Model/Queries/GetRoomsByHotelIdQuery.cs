namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Queries;

/// <summary>
/// Query to retrieve all rooms belonging to a specific hotel.
/// </summary>
/// <param name="HotelId">The unique identifier of the hotel.</param>
public record GetRoomsByHotelIdQuery(int HotelId);
