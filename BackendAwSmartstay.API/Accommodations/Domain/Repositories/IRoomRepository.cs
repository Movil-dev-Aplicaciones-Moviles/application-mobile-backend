using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Accommodations.Domain.Repositories;

/// <summary>
/// Repository interface for managing Room aggregates.
/// </summary>
public interface IRoomRepository : IBaseRepository<Room>
{
    /// <summary>
    /// Retrieves all rooms that belong to a specific hotel.
    /// The filtering is performed at the database level to avoid full table scans.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <returns>A collection of rooms for the specified hotel.</returns>
    Task<IEnumerable<Room>> ListByHotelIdAsync(int hotelId);
}
