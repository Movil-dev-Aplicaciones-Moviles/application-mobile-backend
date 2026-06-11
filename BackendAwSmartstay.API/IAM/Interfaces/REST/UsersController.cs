using System.Net.Mime;
using BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;
using BackendAwSmartstay.API.IAM.Domain.Model.Commands;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;
using BackendAwSmartstay.API.IAM.Domain.Model.Exceptions;
using BackendAwSmartstay.API.IAM.Domain.Model.Queries;
using BackendAwSmartstay.API.IAM.Domain.Services;
using BackendAwSmartstay.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using BackendAwSmartstay.API.IAM.Interfaces.REST.Resources;
using BackendAwSmartstay.API.IAM.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendAwSmartstay.API.IAM.Interfaces.REST;

/// <summary>
///     The user's controller
/// </summary>
/// <remarks>
///     This class is used to handle user requests with full multi-tenancy and scope validation.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available User endpoints")]
public class UsersController(
    IUserQueryService userQueryService,
    IUserCommandService userCommandService) : ControllerBase
{
    /// <summary>
    ///     Creates a new user via management endpoints.
    /// </summary>
    /// <param name="resource">The user creation payload.</param>
    /// <returns>A confirmation message.</returns>
    [HttpPost]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(Summary = "Create a new user", Description = "Creates a user within the actor's hierarchical and organizational scope. Note: Location header implementation pending contract update.", OperationId = "CreateUser")]
    [SwaggerResponse(StatusCodes.Status201Created, "The user was created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request payload or unexpected error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT Token")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have required hierarchy or scope access")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Username already exists")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserResource resource)
    {
        var actor = (User?)HttpContext.Items["User"];
        if (actor == null) return Unauthorized(new { message = "Authentication context is missing or invalid." });

        var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource, actor.Id);

        try
        {
            await userCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created, new { message = "User created successfully" });
        }
        catch (UsernameAlreadyExistsException e)
        {
            return Conflict(new { message = e.Message });
        }
        catch (UnauthorizedOperationException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = e.Message });
        }
        catch (Exception)
        {
            // Masking internal exceptions to prevent information leakage
            return BadRequest(new { message = "An unexpected error occurred while processing your request." });
        }
    }

    /// <summary>
    ///     Get user by id endpoint.
    /// </summary>
    /// <param name="id">The user id to retrieve.</param>
    /// <returns>The enriched user resource.</returns>
    [HttpGet("{id}")]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(Summary = "Get a user by its id", Description = "Retrieves a user only if the actor has scope access to them.", OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT Token")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have required permissions")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found or outside actor's scope")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var actor = (User?)HttpContext.Items["User"];
        if (actor == null) return Unauthorized(new { message = "Authentication context is missing or invalid." });

        var getUserByIdQuery = new GetUserByIdQuery(id, actor.Id);
        var user = await userQueryService.Handle(getUserByIdQuery);

        if (user == null) 
            return NotFound(new { message = "User not found or access denied due to organizational scope." });

        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(userResource);
    }

    /// <summary>
    ///     Get all users' endpoint.
    /// </summary>
    /// <returns>The user resources within the actor's scope.</returns>
    [HttpGet]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(Summary = "Get all users within scope", Description = "Retrieves all users accessible to the authenticated actor.", OperationId = "GetUsersByScope")]
    [SwaggerResponse(StatusCodes.Status200OK, "The scoped users were found", typeof(IEnumerable<UserResource>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT Token")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have required permissions")]
    public async Task<IActionResult> GetAllUsers()
    {
        var actor = (User?)HttpContext.Items["User"];
        if (actor == null) return Unauthorized(new { message = "Authentication context is missing or invalid." });

        var getUsersByScopeQuery = new GetUsersByScopeQuery(actor.Id);
        var users = await userQueryService.Handle(getUsersByScopeQuery);
        
        var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(userResources);
    }

    /// <summary>
    ///     Changes the password for the authenticated user.
    /// </summary>
    /// <param name="resource">The password change resource containing the current and new passwords.</param>
    /// <remarks>UserId is extracted from HttpContext.Items using the established authenticated-user pattern.</remarks>
    /// <returns>An HTTP result indicating whether the password was updated successfully.</returns>
    [HttpPost("change-password")]
    [SwaggerOperation(
        Summary = "Change the authenticated user's password",
        Description = "Requires a valid JWT. UserId is extracted from the token, not from the body.",
        OperationId = "ChangePassword")]
    [SwaggerResponse(StatusCodes.Status200OK, "Password updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data or unexpected error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User is not authenticated or current password incorrect")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The user was not found")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordResource resource)
    {
        var user = (User?)HttpContext.Items["User"];
        if (user == null) return Unauthorized(new { message = "Authentication context is missing or invalid." });

        var command = new ChangePasswordCommand(user.Id, resource.CurrentPassword, resource.NewPassword);
        
        try
        {
            await userCommandService.Handle(command);
            return Ok(new { message = "Password updated successfully" });
        }
        catch (UserNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { message = "Invalid current password" });
        }
        catch (Exception)
        {
            // Masking internal exceptions to prevent information leakage
            return BadRequest(new { message = "An unexpected error occurred while processing your request." });
        }
    }
}