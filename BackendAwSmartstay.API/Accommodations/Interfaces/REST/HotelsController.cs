using System.Net.Mime;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Queries;
using BackendAwSmartstay.API.Accommodations.Domain.Model.ValueObjects;
using BackendAwSmartstay.API.Accommodations.Domain.Services;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Resources;
using BackendAwSmartstay.API.Accommodations.Interfaces.REST.Transform;
using BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;
using BackendAwSmartstay.API.IAM.Domain.Model.Constants;
using BackendAwSmartstay.API.IAM.Interfaces.ACL;
using BackendAwSmartstay.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BackendAwSmartstay.API.Accommodations.Interfaces.REST;

/// <summary>
///     RESTful API interface controller responsible for handling corporate and guest operations 
///     related to hotel property aggregates within the hotel accommodation bounded context.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Hotel Endpoints")]
public class HotelsController(
    IHotelCommandService hotelCommandService,
    IHotelQueryService hotelQueryService,
    IHotelPricingService hotelPricingService,
    IRoomQueryService roomQueryService,
    IIamContextFacade iamContextFacade) : ControllerBase
{
    /// <summary>
    ///     Retrieves a collection of all registered hotel property resources.
    /// </summary>
    /// <remarks>
    ///     Accessible by all verified roles (Guests, Admins, and ChainAdmins) across client applications.
    /// </remarks>
    /// <returns>An asynchronous action result containing an enumerable collection of hotel representations.</returns>
    [HttpGet]
    [Authorize(UserRoles.Guest, UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Get all hotels",
        Description = "Retrieves hotel aggregates based on role-based data isolation. ChainAdmin sees all hotels; Admin sees only their assigned hotel; operational roles are denied.",
        OperationId = "GetAllHotels")]
    [SwaggerResponse(StatusCodes.Status200OK, "The hotel resource list was successfully fetched.", typeof(IEnumerable<HotelResource>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated identity has insufficient privilege levels.")]
    public async Task<IActionResult> GetAllHotels()
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
            return Unauthorized();

        var role = user.Role.Value?.ToLowerInvariant();

        // REGLA 1: ChainAdmin sees all hotels
        if (role == UserRoles.ChainAdmin)
        {
            var hotels = await hotelQueryService.Handle(new GetAllHotelsQuery());
            var resources = new List<HotelResource>();

            // SOLUCIÓN CRÍTICA EF CORE: Iteración secuencial asíncrona para evitar hilos concurrentes compartiendo el mismo DbContext
            foreach (var hotel in hotels)
            {
                resources.Add(await BuildHotelResourceAsync(hotel));
            }
            return Ok(resources);
        }

        // REGLA 2: Admin sees only their assigned hotel
        if (role == UserRoles.Admin)
        {
            if (user.HotelId == null)
                return Forbid();

            var hotel = await hotelQueryService.Handle(new GetHotelByIdQuery(user.HotelId.Value));
            if (hotel == null)
                return NotFound();

            var resource = await BuildHotelResourceAsync(hotel);
            return Ok(new List<HotelResource> { resource });
        }

        // REGLA 3: Operational roles (reception, housekeeping, maintenance, staff) are NOT allowed to see hotels
        if (role == UserRoles.Staff || role == UserRoles.Reception ||
            role == UserRoles.Housekeeping || role == UserRoles.Maintenance)
        {
            return Forbid();
        }

        // GUEST: allowed to see all hotels for browsing
        if (role == UserRoles.Guest)
        {
            var hotels = await hotelQueryService.Handle(new GetAllHotelsQuery());
            var resources = new List<HotelResource>();

            // SOLUCIÓN CRÍTICA EF CORE: Mismo parche secuencial aplicado al scope de invitados
            foreach (var hotel in hotels)
            {
                resources.Add(await BuildHotelResourceAsync(hotel));
            }
            return Ok(resources);
        }

        return Forbid();
    }

    /// <summary>
    ///     Retrieves a unique hotel property resource by its technical identity marker.
    /// </summary>
    /// <param name="hotelId">The structural domain identity number of the hotel target aggregate.</param>
    /// <returns>The matching hotel representation resource context, or NotFound.</returns>
    [HttpGet("{hotelId:int}")]
    [Authorize(UserRoles.Guest, UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Get hotel by its unique identifier",
        Description = "Retrieves structural property details for a single hotel aggregate from its domain identifier.",
        OperationId = "GetHotelById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The hotel aggregate was located and mapped successfully.", typeof(HotelResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated identity has insufficient privilege levels.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No hotel aggregate matched the supplied structural query identifier.")]
    public async Task<IActionResult> GetHotelById(int hotelId)
    {
        var hotel = await hotelQueryService.Handle(new GetHotelByIdQuery(hotelId));
        if (hotel is null) return NotFound();
        var resource = await BuildHotelResourceAsync(hotel);
        return Ok(resource);
    }

    /// <summary>
    ///     Creates a new hotel aggregate root inside the transactional bounded context.
    /// </summary>
    /// <param name="resource">The incoming payload representation mapping properties required for construction.</param>
    /// <returns>A created resource location confirmation with the persistence tracking instance representation.</returns>
    [HttpPost]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Create a new hotel property entry",
        Description = "Constructs a new hotel aggregate root. Restricted exclusively to administrative and corporate management roles.",
        OperationId = "CreateHotel")]
    [SwaggerResponse(StatusCodes.Status201Created, "The hotel aggregate root was successfully created and tracked.", typeof(HotelResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided construction resource structure contains invalid constraints.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied. Only Admin or ChainAdmin operators are cleared to execute infrastructure initialization.")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelResource resource)
    {
        var command = CreateHotelCommandFromResourceAssembler.ToCommandFromResource(resource);
        var hotel = await hotelCommandService.Handle(command);
        
        if (hotel is null) return BadRequest();
        
        var hotelResource = await BuildHotelResourceAsync(hotel);
        return CreatedAtAction(nameof(GetHotelById), new { hotelId = hotel.Id }, hotelResource);
    }
    
    /// <summary>
    ///     Updates the state representation parameters of a registered hotel aggregate root.
    /// </summary>
    /// <param name="hotelId">The structural identifier pointing to the aggregate instance undergoing mutation.</param>
    /// <param name="resource">The incoming state modification layout resource constraints.</param>
    /// <returns>The updated hotel resource state outcome representation.</returns>
    [HttpPut("{hotelId:int}")]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Update an existing hotel aggregate's context properties",
        Description = "Mutates descriptive fields on an active hotel target. Only accessible by authorized management nodes.",
        OperationId = "UpdateHotel")]
    [SwaggerResponse(StatusCodes.Status200OK, "The hotel aggregate state was updated successfully.", typeof(HotelResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated identity has insufficient administrative privilege levels.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The targeted hotel aggregate could not be extracted for state alteration.")]
    public async Task<IActionResult> UpdateHotel(int hotelId, [FromBody] UpdateHotelResource resource)
    {
        var command = UpdateHotelCommandFromResourceAssembler.ToCommandFromResource(hotelId, resource);
        var updatedHotel = await hotelCommandService.Handle(command);

        if (updatedHotel is null) return NotFound();

        var hotelResource = await BuildHotelResourceAsync(updatedHotel);
        return Ok(hotelResource);
    }

    /// <summary>
    ///     Removes an active hotel property aggregate from the domain persistence subsystem.
    /// </summary>
    /// <param name="hotelId">The unique structural aggregate identifier targeted for transactional removal.</param>
    /// <returns>The final detached state representation of the processed resource entry.</returns>
    [HttpDelete("{hotelId:int}")]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Delete a hotel property cluster",
        Description = "Triggers complete cascading teardown routines for a single hotel entity group. Strictly for administrative clearance nodes.",
        OperationId = "DeleteHotel")]
    [SwaggerResponse(StatusCodes.Status200OK, "The hotel property aggregate hierarchy was completely processed and decommissioned.", typeof(HotelResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The requesting identity lacks the extreme corporate validation depth to delete property nodes.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The targeted hotel index node was not present in the structural cluster tree.")]
    public async Task<IActionResult> DeleteHotel(int hotelId)
    {
        var command = new DeleteHotelCommand(hotelId);
        var deletedHotel = await hotelCommandService.Handle(command);

        if (deletedHotel is null) return NotFound();

        var hotelResource = await BuildHotelResourceAsync(deletedHotel);
        return Ok(hotelResource);
    }

    /// <summary>
    ///     Retrieves operational metrics for a specific hotel.
    ///     Only accessible by Admin (own hotel) or ChainAdmin (any hotel).
    /// </summary>
    /// <param name="hotelId">The unique identifier of the target hotel.</param>
    /// <returns>A hotel metrics resource containing staff count, room totals, and room status breakdowns.</returns>
    [HttpGet("{hotelId:int}/metrics")]
    [Authorize(UserRoles.Admin, UserRoles.ChainAdmin)]
    [SwaggerOperation(
        Summary = "Get hotel operational metrics",
        Description = "Returns a summary of operational KPIs including staff count, total rooms, and room status breakdowns. Access is isolated to the user's assigned hotel unless the user is ChainAdmin.",
        OperationId = "GetHotelMetrics")]
    [SwaggerResponse(StatusCodes.Status200OK, "The hotel metrics were calculated successfully.", typeof(HotelMetricsResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "The request lacks a valid identity identification token.")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated identity cannot access metrics for this hotel.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No hotel matched the supplied identifier.")]
    public async Task<IActionResult> GetHotelMetrics(int hotelId)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
            return Unauthorized();

        var role = user.Role.Value?.ToLowerInvariant();

        // ChainAdmin can see metrics for any hotel
        if (role == UserRoles.ChainAdmin)
        {
            return await BuildMetricsResponse(hotelId);
        }

        // Admin: MUST validate that the requested hotelId matches their own
        if (role == UserRoles.Admin)
        {
            if (user.HotelId == null || user.HotelId.Value != hotelId)
                return Forbid();

            return await BuildMetricsResponse(hotelId);
        }

        return Forbid();
    }

    /// <summary>
    /// Helper method to convert a Hotel entity to a HotelResource with its lowest price.
    /// </summary>
    private async Task<HotelResource> BuildHotelResourceAsync(Hotel hotel)
    {
        var lowestPrice = await hotelPricingService.CalculateLowestPriceAsync(hotel.Id);
        return HotelResourceFromEntityAssembler.ToResourceFromEntity(hotel, lowestPrice);
    }

    private async Task<IActionResult> BuildMetricsResponse(int hotelId)
    {
        var hotel = await hotelQueryService.Handle(new GetHotelByIdQuery(hotelId));
        if (hotel == null)
            return NotFound();

        // 1. Total staff assigned to this hotel (via IAM ACL to preserve bounded context isolation)
        var totalStaff = await iamContextFacade.GetStaffCountByHotelIdAsync(hotelId);

        // 2. All rooms for this hotel
        var rooms = await roomQueryService.Handle(new GetRoomsByHotelIdQuery(hotelId));
        var roomsList = rooms.ToList();
        var totalRooms = roomsList.Count;

        // 3. Rooms grouped by status (strongly typed via RoomStatus)
        var cleanRooms = roomsList.Count(r => r.Status == RoomStatus.Clean);
        var dirtyRooms = roomsList.Count(r => r.Status == RoomStatus.Dirty);
        var maintenanceRooms = roomsList.Count(r => r.Status == RoomStatus.Maintenance);
        var reservedRooms = roomsList.Count(r => r.Status == RoomStatus.Reserved);

        var metrics = new HotelMetricsResource(
            hotelId,
            hotel.Name,
            totalStaff,
            totalRooms,
            cleanRooms,
            dirtyRooms,
            maintenanceRooms,
            reservedRooms
        );

        return Ok(metrics);
    }
}