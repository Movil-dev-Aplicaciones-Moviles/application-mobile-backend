using BackendAwSmartstay.API.Shared.Domain.Model.Events;
using BackendAwSmartstay.API.Shared.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Cortex.Mediator; 

namespace BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(AppDbContext context, IMediator mediator) : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task CompleteAsync()
    {
        var domainEvents = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IEntityWithEvents entityWithEvents && entityWithEvents.DomainEvents.Any())
            .SelectMany(e => ((IEntityWithEvents)e.Entity).DomainEvents)
            .ToList();
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IEntityWithEvents entityWithEvents)
            {
                entityWithEvents.ClearDomainEvents();
            }
        }
        
        foreach (var domainEvent in domainEvents)
        {
            await mediator.PublishAsync(domainEvent, CancellationToken.None);
        }
        
        await context.SaveChangesAsync();
    }
}