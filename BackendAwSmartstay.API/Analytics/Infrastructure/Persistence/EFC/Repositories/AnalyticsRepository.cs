using BackendAwSmartstay.API.Analytics.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Analytics.Domain.Repositories;
using BackendAwSmartstay.API.Bookings.Domain.Model.Aggregates;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BackendAwSmartstay.API.Analytics.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Provides an Entity Framework Core implementation for retrieving analytical metrics.
/// </summary>
/// <remarks>
/// This repository acts as a cross-context reader that queries information from multiple
/// domain bounded contexts (Payments, Bookings, and Accommodations) to build analytical aggregates.
/// </remarks>
/// <param name="context">The database context instance for database operations.</param>
public class AnalyticsRepository(AppDbContext context) : IAnalyticsRepository
{
    /// <summary>
    /// Computes and retrieves aggregated performance metrics for the current calendar month.
    /// </summary>
    /// <remarks>
    /// Calculations are strictly bounded between the first second of the current month and the first second of the next month (exclusive) 
    /// using UTC time to prevent timezone shifts. 
    /// Financial records are safely aggregated directly via the root Value Object property conversion. Room occupancy calculations 
    /// are evaluated in memory to safeguard against division-by-zero exceptions in the database engine.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the populated <see cref="PerformanceMetrics"/> domain instance.
    /// </returns>
    public async Task<PerformanceMetrics> GetMonthlyMetricsAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonth = startOfMonth.AddMonths(1);

        // 1. Calculate Revenue safely using decimal casting on the Root Value Object property to prevent Linq translation failures
        var totalRevenue = await context.Set<Payments.Domain.Model.Aggregates.Payment>()
            .Where(p => p.PaymentDate >= startOfMonth && p.PaymentDate < nextMonth && p.Status == Payments.Domain.Model.Aggregates.PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.AmountRecord) ?? 0m;

        // 2. Base query for tracking bookings within the current monthly window
        var bookingsQuery = context.Set<Booking>()
            .Where(b => b.CheckInDate >= startOfMonth && b.CheckInDate < nextMonth);

        var totalBookings = await bookingsQuery.CountAsync();
    
        var cancelledBookings = await bookingsQuery
            .CountAsync(b => b.Status == BookingStatus.Cancelled);

        // 3. Gather foundational metrics for occupancy calculations
        var totalRooms = await context.Set<Accommodations.Domain.Model.Aggregates.Room>().CountAsync();
        var activeBookings = await bookingsQuery.CountAsync(b => b.Status == BookingStatus.Confirmed);
    
        double occupancyRate = 0.0;
    
        // 4. Domain heuristic protection against division by zero errors
        if (totalRooms > 0 && totalBookings > 0)
        {
            occupancyRate = ((double)activeBookings / totalRooms) * 100;
        }

        return new PerformanceMetrics
        {
            TotalRevenue = totalRevenue,
            TotalBookings = totalBookings,
            CancelledBookings = cancelledBookings,
            OccupancyRate = Math.Round(occupancyRate, 2)
        };
    }
}