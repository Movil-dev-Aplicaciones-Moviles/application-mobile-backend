namespace BackendAwSmartstay.API.Accommodations.Domain.Services;

/// <summary>
/// Domain service interface for calculating hotel pricing information.
/// This service replaces the previous navigation-property-based approach,
/// enforcing aggregate boundary compliance by querying rooms through
/// the repository instead of traversing a direct collection.
/// </summary>
public interface IHotelPricingService
{
    /// <summary>
    /// Calculates the lowest room price for the specified hotel.
    /// </summary>
    /// <param name="hotelId">The unique identifier of the hotel.</param>
    /// <returns>The minimum price among all rooms in the hotel, or 0 if no rooms exist.</returns>
    Task<decimal> CalculateLowestPriceAsync(int hotelId);
}
