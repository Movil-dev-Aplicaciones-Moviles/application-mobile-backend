using System.Net.Mime;
using BackendAwSmartstay.API.Analytics.Domain.Model.Queries;
using BackendAwSmartstay.API.Analytics.Domain.Services;
using BackendAwSmartstay.API.Analytics.Interfaces.REST.Resources;
using BackendAwSmartstay.API.Analytics.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendAwSmartstay.API.Analytics.Interfaces.REST;

/// <summary>
///     REST controller for analytics operations.
/// </summary>
//[Authorize(UserRoles.Admin, UserRoles.ChainAdmin)] // Only Admin/ChainAdmin should access this
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Analytics Endpoints")]
public class AnalyticsController(IAnalyticsQueryService analyticsQueryService, IConnectionMultiplexer redis)
    : ControllerBase
{
    private readonly IDatabase _db = redis.GetDatabase();

    /// <summary>
    ///     Retrieves monthly performance metrics.
    /// </summary>
    /// <returns>An action result containing the performance metrics resource.</returns>
    [HttpGet("performance/monthly")]
    [SwaggerOperation(
        Summary = "Get monthly performance metrics",
        Description = "Retrieves aggregated metrics like revenue and occupancy for the current month.",
        OperationId = "GetMonthlyPerformance")]
    [SwaggerResponse(StatusCodes.Status200OK, "The metrics", typeof(PerformanceMetricsResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT Token")]
    [SwaggerResponse(StatusCodes.Status403Forbidden,
        "User does not have required permissions (Requires Admin/ChainAdmin)")]
    public async Task<IActionResult> GetMonthlyPerformance()
    {
        var query = new GetMonthlyPerformanceQuery();
        var metrics = await analyticsQueryService.Handle(query);
        var resource = PerformanceMetricsAssembler.ToResourceFromEntity(metrics);
        return Ok(resource);
    }

    [HttpPost("cache")]
    public async Task<IActionResult> CacheData([FromBody] string message)
    {
        await _db.ListRightPushAsync("analytics-messages", message);

        return Ok(new
        {
            success = true,
            saved = message
        });
    }

    [HttpGet("cache")]
    public async Task<IActionResult> GetCache()
    {
        var values = await _db.ListRangeAsync("analytics-messages");

        return Ok(values.Select(v => v.ToString()));
    }
}