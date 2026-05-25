namespace BackendAwSmartstay.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a successfully authenticated user response.
/// </summary>
/// <param name="Id">The user's unique identifier.</param>
/// <param name="Username">The user's username.</param>
/// <param name="Token">The JWT access token.</param>
public record AuthenticatedUserResource(int Id, string Username, string Token);
