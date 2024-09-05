using System.Collections.Concurrent;
using TestTask.Application.Common.Interfaces;
using TestTask.Application.Models.Common;
using TestTask.Application.Models.RequestModels;

namespace TestTask.Infrastructure.Services;

public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<Guid, Route> _cache = new();

    public void AddRoutesToCache(IEnumerable<Route> routes)
    {
        foreach (var route in routes)
        {
            if (route.TimeLimit > DateTime.UtcNow)
            {
                _cache[route.Id] = route;
            }
        }
    }

    public IEnumerable<Route> GetRoutesFromCache(SearchRequest request)
    {
        return _cache.Values
            .Where(r => r.Origin == request.Origin && r.Destination == request.Destination)
            .ToList();
    }
}
