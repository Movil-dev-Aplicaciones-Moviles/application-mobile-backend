using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace BackendAwSmartstay.API.Accommodations.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Repository for HousekeepingTask aggregates.
/// Inherits standard CRUD operations from <see cref="BaseRepository{TEntity}"/>.
/// </summary>
public class HousekeepingTaskRepository(AppDbContext context)
    : BaseRepository<HousekeepingTask>(context), IHousekeepingTaskRepository
{
}
