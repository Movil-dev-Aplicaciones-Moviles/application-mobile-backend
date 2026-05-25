namespace BackendAwSmartstay.API.IAM.Domain.Model.Commands;

/// <summary>
///     Command to register a new user.
/// </summary>
/// <param name="Username">The desired username.</param>
/// <param name="Password">The raw password.</param>
public record SignUpCommand(string Username, string Password);
