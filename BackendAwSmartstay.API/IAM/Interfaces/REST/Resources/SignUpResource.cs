using System.ComponentModel.DataAnnotations;

namespace BackendAwSmartstay.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Resource definition for user registration data.
/// </summary>
/// <param name="Username">The desired username (email).</param>
/// <param name="Password">The desired password.</param>
/// <param name="Role">The optional role to assign.</param>
public record SignUpResource(
    [Required] string Username, 
    [Required] string Password,
    string? Role
);