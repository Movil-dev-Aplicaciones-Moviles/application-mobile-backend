using BackendAwSmartstay.API.IAM.Domain.Model.Commands;
using BackendAwSmartstay.API.IAM.Interfaces.REST.Resources;

namespace BackendAwSmartstay.API.IAM.Interfaces.REST.Transform;

/// <summary>
///     Assembler to convert a SignUpResource into a SignUpCommand.
/// </summary>
public static class SignUpCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts the resource to a domain command.
    /// </summary>
    /// <param name="resource">The sign-up resource.</param>
    /// <returns>The command for user registration.</returns>
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        return new SignUpCommand(resource.Username, resource.Password);
    }
}