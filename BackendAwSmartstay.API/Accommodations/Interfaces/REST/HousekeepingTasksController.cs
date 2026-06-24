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
/// RESTful API interface controller for managing housekeeping task operations.
/// Provides endpoints to create and complete cleaning tasks.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/housekeeping-tasks")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Housekeeping Task Endpoints")]
public class HousekeepingTasksController(
    IHousekeepingCommandService housekeepingCommandService) : ControllerBase
{
    /// <summary>
    /// Creates a new housekeeping task for a specific room.
    /// </summary>
    /// <param name="resource">The resource containing task creation data.</param>
    /// <returns>The created housekeeping task resource.</returns>
    [HttpPost]
    [Authorize(UserRoles.Admin, UserRoles.Housekeeping)]
    [SwaggerOperation(
        Summary = "Create a new housekeeping task",
        Description = "Creates a cleaning task for a room. Accessible by Admin and Housekeeping roles.",
        OperationId = "CreateHousekeepingTask")]
    [SwaggerResponse(StatusCodes.Status201Created, "The housekeeping task was created successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request payload.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges.")]
    public async Task<IActionResult> CreateHousekeepingTask([FromBody] CreateHousekeepingTaskResource resource)
    {
        var command = CreateHousekeepingTaskCommandFromResourceAssembler.ToCommandFromResource(resource);
        var task = await housekeepingCommandService.Handle(command);
        if (task is null) return BadRequest();
        return CreatedAtAction(nameof(CreateHousekeepingTask), new { taskId = task.Id }, task);
    }

    /// <summary>
    /// Marks an existing housekeeping task as completed.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task to complete.</param>
    /// <returns>The completed task or NotFound if the task does not exist.</returns>
    [HttpPost("{taskId:int}/complete")]
    [Authorize(UserRoles.Housekeeping, UserRoles.Admin)]
    [SwaggerOperation(
        Summary = "Complete a housekeeping task",
        Description = "Marks the task as completed. The Room aggregate will automatically update its status to Clean via domain event.",
        OperationId = "CompleteHousekeepingTask")]
    [SwaggerResponse(StatusCodes.Status200OK, "The task was completed successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No task matched the supplied identifier.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Insufficient privileges.")]
    public async Task<IActionResult> CompleteHousekeepingTask(int taskId)
    {
        var command = new CompleteHousekeepingTaskCommand(taskId);
        var task = await housekeepingCommandService.Handle(command);
        if (task is null) return NotFound();
        return Ok(task);
    }
}
