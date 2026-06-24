using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Accommodations.Domain.Services;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.QueryServices;

/// <summary>
/// Implements the domain pricing service by querying rooms through
/// the repository, preserving aggregate boundary isolation.
/// </summary>
public class HotelPricingService(IRoomRepository roomRepository) : IHotelPricingService
{
    /// <inheritdoc />
    public async Task<decimal> CalculateLowestPriceAsync(int hotelId)
    {
        // Database-level filter via WHERE hotel_id = @hotelId — no full table scan
        var hotelRooms = await roomRepository.ListByHotelIdAsync(hotelId);
        
        if (!hotelRooms.Any())
            return 0;
        
        return hotelRooms.Min(r => r.Price);
    }
}
