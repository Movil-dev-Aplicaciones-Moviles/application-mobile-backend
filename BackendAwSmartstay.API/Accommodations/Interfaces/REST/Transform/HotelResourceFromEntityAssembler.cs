using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;

namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST.Transform;

public static class HotelResourceFromEntityAssembler
{
    /// <summary>
    /// Converts a Hotel entity to a HotelResource.
    /// The lowestPrice is received as a parameter (computed via IHotelPricingService)
    /// to avoid coupling the Hotel aggregate with Room data.
    /// </summary>
    /// <param name="entity">The hotel entity.</param>
    /// <param name="lowestPrice">The pre-computed lowest room price for this hotel.</param>
    /// <returns>A HotelResource ready for the REST response.</returns>
    public static HotelResource ToResourceFromEntity(Hotel entity, decimal lowestPrice)
    {
        var locationDisplay = $"{entity.Address}, {entity.City}, {entity.Country}";

        return new HotelResource(
            entity.Id,
            entity.HostId,
            entity.Name,
            locationDisplay, 
            entity.ImageUrl,
            entity.Description,
            lowestPrice,
            entity.Type,
            entity.Amenities
        );
    }
}