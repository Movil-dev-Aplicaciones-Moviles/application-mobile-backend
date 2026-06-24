using System.Net.Mime;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Services;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Transform;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;
using BackendAwSmartstay.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST;

/// <summary>
/// RESTful API interface controller for managing maintenance request operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/maintenance-requests")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Maintenance Request Endpoints")]
public class MaintenanceRequestsController(
    IMaintenanceCommandService maintenanceCommandService) : ControllerBase
{
    /// <summary>
    /// Creates a new maintenance request for a specific room.
    /// </summary>
    [HttpPost]
    [Authorize(UserRoles.Admin, UserRoles.Reception, UserRoles.Housekeeping)]
    [SwaggerOperation(
        Summary = "Create a new maintenance request",
        Description = "Creates a maintenance request for a room. Accessible by Admin, Reception, and Housekeeping roles.",
        OperationId = "CreateMaintenanceRequest")]
    [SwaggerResponse(StatusCodes.Status201Created, "The maintenance request was created successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request payload.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges.")]
    public async Task<IActionResult> CreateMaintenanceRequest([FromBody] CreateMaintenanceRequestResource resource)
    {
        var command = CreateMaintenanceRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
        var request = await maintenanceCommandService.Handle(command);
        if (request is null) return BadRequest();
        return CreatedAtAction(nameof(CreateMaintenanceRequest), new { requestId = request.Id }, request);
    }

    /// <summary>
    /// Starts maintenance work on an existing request.
    /// </summary>
    [HttpPost("{requestId:int}/start")]
    [Authorize(UserRoles.Maintenance, UserRoles.Admin)]
    [SwaggerOperation(
        Summary = "Start maintenance work",
        Description = "Marks the request as in-progress. The Room aggregate will automatically update its status to 'Maintenance' via domain event.",
        OperationId = "StartMaintenance")]
    [SwaggerResponse(StatusCodes.Status200OK, "The maintenance work has started.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No request matched the supplied identifier.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges.")]
    public async Task<IActionResult> StartMaintenance(int requestId, [FromBody] int technicianId)
    {
        var command = new StartMaintenanceCommand(requestId, technicianId);
        var request = await maintenanceCommandService.Handle(command);
        if (request is null) return NotFound();
        return Ok(request);
    }

    /// <summary>
    /// Resolves an existing maintenance request.
    /// </summary>
    [HttpPost("{requestId:int}/resolve")]
    [Authorize(UserRoles.Maintenance, UserRoles.Admin)]
    [SwaggerOperation(
        Summary = "Resolve maintenance work",
        Description = "Marks the request as resolved. The Room aggregate will automatically update its status to 'Dirty' via domain event, requiring housekeeping.",
        OperationId = "ResolveMaintenance")]
    [SwaggerResponse(StatusCodes.Status200OK, "The maintenance work was resolved.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No request matched the supplied identifier.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges.")]
    public async Task<IActionResult> ResolveMaintenance(int requestId)
    {
        var command = new ResolveMaintenanceCommand(requestId);
        var request = await maintenanceCommandService.Handle(command);
        if (request is null) return NotFound();
        return Ok(request);
    }
}
