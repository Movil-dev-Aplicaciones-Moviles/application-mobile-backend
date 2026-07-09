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
            ResolvePublicImageUrl(entity),
            entity.Description,
            lowestPrice,
            entity.Type,
            entity.Amenities
        );
    }

    /// <summary>
    /// Returns a public image URL suitable for the client app.
    /// If the database still contains old placeholder images, the API response uses a realistic image instead.
    /// This avoids requiring a database migration just to improve the visual catalog.
    /// </summary>
    private static string ResolvePublicImageUrl(Hotel entity)
    {
        if (!string.IsNullOrWhiteSpace(entity.ImageUrl) &&
            !entity.ImageUrl.Contains("placehold.co", StringComparison.OrdinalIgnoreCase))
        {
            return entity.ImageUrl;
        }

        return entity.Name.ToLowerInvariant().Contains("cusco")
            ? "https://images.unsplash.com/photo-1531968455001-5c5272a41129?auto=format&fit=crop&w=1200&q=80"
            : "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80";
    }
}