using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Shared.Domain.Repositories;

namespace BackendAwSmartstay.API.Accommodations.Domain.Repositories;

/// <summary>
/// Repository interface for managing HousekeepingTask aggregates.
/// </summary>
public interface IHousekeepingTaskRepository : IBaseRepository<HousekeepingTask>
{
}
