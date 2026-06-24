namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

/// <summary>
/// Represents a summary of operational metrics for a specific hotel,
/// intended for the Admin dashboard.
/// </summary>
/// <param name="HotelId">The unique identifier of the hotel.</param>
/// <param name="HotelName">The display name of the hotel.</param>
/// <param name="TotalStaff">Total number of active staff members assigned to this hotel.</param>
/// <param name="TotalRooms">Total number of rooms in this hotel.</param>
/// <param name="CleanRooms">Number of rooms in Clean status.</param>
/// <param name="DirtyRooms">Number of rooms in Dirty status.</param>
/// <param name="MaintenanceRooms">Number of rooms in Maintenance status.</param>
/// <param name="ReservedRooms">Number of rooms in Reserved status.</param>
public record HotelMetricsResource(
    int HotelId,
    string HotelName,
    int TotalStaff,
    int TotalRooms,
    int CleanRooms,
    int DirtyRooms,
    int MaintenanceRooms,
    int ReservedRooms
);
