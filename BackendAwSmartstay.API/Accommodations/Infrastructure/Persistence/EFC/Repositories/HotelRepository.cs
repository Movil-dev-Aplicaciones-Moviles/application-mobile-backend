using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackendAwSmartstay.API.Accommodations.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Repository for handling Hotel aggregate persistence.
/// Overrides standard behavior to include related Room data for pricing calculations.
/// </summary>
public class HotelRepository(AppDbContext context) : BaseRepository<Hotel>(context), IHotelRepository
{
    // Repository inherits ListAsync and FindByIdAsync from BaseRepository<Hotel>.
    // No .Include(h => h.Rooms) is used here because Hotel and Room are separate aggregates.
    // Room data is accessed exclusively through IRoomRepository to preserve aggregate boundaries.
}