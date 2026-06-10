namespace BackendAwSmartstay.API.IAM.Domain.Model.Queries;

/// <summary>
///     The get all users query
/// </summary>
/// <remarks>
///     This query object is used to get all users
/// </remarks>
[Obsolete("This query does not enforce organizational scope and may cause data leaks. Use GetUsersByScopeQuery instead. Slated for removal.", false)]
public record GetAllUsersQuery;