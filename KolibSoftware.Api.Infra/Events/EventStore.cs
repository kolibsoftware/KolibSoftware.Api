using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;

namespace KolibSoftware.Api.Infra.Events;

public sealed class EventStore(
    IQueryableRepository<EventModel> repository
) : IEventStore
{
    public async Task PutEventsAsync(IEnumerable<EventModel> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            if (@event.Id == 0) await repository.InsertAsync(@event, cancellationToken);
            else await repository.UpdateAsync(@event, cancellationToken);
        }
    }

    public async Task<IEnumerable<EventModel>> GetEventsAsync(IQuery<EventModel> query, CancellationToken cancellationToken = default)
    {
        var events = await repository.GetAllAsync(query, cancellationToken);
        return events;
    }

}