using TestTask.Application.Models.Common;
using TestTask.Application.Models.RequestModels;

namespace TestTask.Application.Common.Interfaces;

public interface ICacheService
{
    void AddRoutesToCache(IEnumerable<Route> routes);
    IEnumerable<Route> GetRoutesFromCache(SearchRequest request);
}
