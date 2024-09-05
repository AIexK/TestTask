using TestTask.Application.Models.RequestModels;
using TestTask.Application.Models.ResponseModels;

namespace TestTask.Application.Common.Interfaces;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}
