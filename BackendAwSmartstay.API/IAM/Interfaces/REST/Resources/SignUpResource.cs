using System.ComponentModel.DataAnnotations;

namespace BackendAwSmartstay.API.IAM.Interfaces.REST.Resources;

/// <summary>
///     Resource definition for user registration data.
/// </summary>
/// <param name="Username">The desired username (email).</param>
/// <param name="Password">The desired password.</param>
public record SignUpResource(
    [Required] string Username, 
    [Required] string Password
);