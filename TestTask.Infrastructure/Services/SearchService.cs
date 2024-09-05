using System.Net.Http.Json;
using TestTask.Application.Common.Interfaces;
using TestTask.Application.Models.Common;
using TestTask.Application.Models.RequestModels;
using TestTask.Application.Models.ResponseModels;

namespace TestTask.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly ICacheService _cacheService;

    // В реальном приложении следует вынести в appsettings
    private const string PROVIDER_ONE_SEARCH_URL = "http://provider-one/api/v1/search";
    private const string PROVIDER_TWO_SEARCH_URL = "http://provider-two/api/v1/search";

    private const string PROVIDER_ONE_PING_URL = "http://provider-one/api/v1/ping";
    private const string PROVIDER_TWO_PING_URL = "http://provider-two/api/v1/ping";


    public SearchService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        if (request.Filters?.OnlyCached == true)
        {
            return GetCachedRoutes(request);
        }

        var routes = new List<Route>();

        bool providerOneAvailable = await IsProviderAvailableAsync(PROVIDER_ONE_PING_URL, cancellationToken);
        bool providerTwoAvailable = await IsProviderAvailableAsync(PROVIDER_TWO_PING_URL, cancellationToken);

        if (providerOneAvailable)
        {
            routes.AddRange(await SearchProviderOneAsync(request, cancellationToken));
        }

        if (providerTwoAvailable)
        {
            routes.AddRange(await SearchProviderTwoAsync(request, cancellationToken));
        }

        _cacheService.AddRoutesToCache(routes);

        var filteredRoutes = FilterRoutes(routes, request.Filters);

        return CreateSearchResponse(filteredRoutes);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        var providerOneAvailable = await IsProviderAvailableAsync(PROVIDER_ONE_PING_URL, cancellationToken);
        var providerTwoAvailable = await IsProviderAvailableAsync(PROVIDER_TWO_PING_URL, cancellationToken);
        return providerOneAvailable || providerTwoAvailable;
    }

    private async Task<bool> IsProviderAvailableAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<IEnumerable<Route>> SearchProviderOneAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var providerRequest = new ProviderOneSearchRequest
        {
            From = request.Origin,
            To = request.Destination,
            DateFrom = request.OriginDateTime,
            DateTo = request.Filters?.DestinationDateTime,
            MaxPrice = request.Filters?.MaxPrice
        };

        var httpClient = new HttpClient();
        var response = await httpClient
            .PostAsJsonAsync(PROVIDER_TWO_SEARCH_URL, providerRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)            
            return Enumerable.Empty<Route>();

        var providerResponse = await response.Content
            .ReadFromJsonAsync<ProviderOneSearchResponse>(cancellationToken: cancellationToken);

        return providerResponse.Routes.Select(r => new Route
        {
            Id = Guid.NewGuid(),
            Origin = r.From,
            Destination = r.To,
            OriginDateTime = r.DateFrom,
            DestinationDateTime = r.DateTo,
            Price = r.Price,
            TimeLimit = r.TimeLimit
        });
    }

    private async Task<IEnumerable<Route>> SearchProviderTwoAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var providerRequest = new ProviderTwoSearchRequest
        {
            Departure = request.Origin,
            Arrival = request.Destination,
            DepartureDate = request.OriginDateTime,
            MinTimeLimit = request.Filters?.MinTimeLimit
        };

        var httpClient = new HttpClient();
        var response = await httpClient
            .PostAsJsonAsync(PROVIDER_TWO_SEARCH_URL, providerRequest, cancellationToken);

        if (!response.IsSuccessStatusCode) 
            return Enumerable.Empty<Route>();

        var providerResponse = await response.Content
            .ReadFromJsonAsync<ProviderTwoSearchResponse>(cancellationToken: cancellationToken);

        return providerResponse.Routes.Select(r => new Route
        {
            Id = Guid.NewGuid(),
            Origin = r.Departure.Point,
            Destination = r.Arrival.Point,
            OriginDateTime = r.Departure.Date,
            DestinationDateTime = r.Arrival.Date,
            Price = r.Price,
            TimeLimit = r.TimeLimit
        });
    }

    private SearchResponse GetCachedRoutes(SearchRequest request)
    {
        var cachedRoutes = _cacheService.GetRoutesFromCache(request);
        var filteredRoutes = FilterRoutes(cachedRoutes, request.Filters);
        return CreateSearchResponse(filteredRoutes);
    }

    private List<Route> FilterRoutes(IEnumerable<Route> routes, SearchFilters filters)
    {
        if (filters == null) return routes.ToList();

        return routes
            .Where(r => !filters.MaxPrice.HasValue || r.Price <= filters.MaxPrice.Value)
            .Where(r => !filters.DestinationDateTime.HasValue || r.DestinationDateTime <= filters.DestinationDateTime.Value)
            .Where(r => !filters.MinTimeLimit.HasValue || r.TimeLimit >= filters.MinTimeLimit.Value)
            .ToList();
    }

    private SearchResponse CreateSearchResponse(IEnumerable<Route> routes)
    {
        var routeList = routes.ToList();

        return new SearchResponse
        {
            Routes = routeList.ToArray(),
            MinPrice = routeList.Min(r => r.Price),
            MaxPrice = routeList.Max(r => r.Price),
            MinMinutesRoute = routeList.Min(r => (int)(r.DestinationDateTime - r.OriginDateTime).TotalMinutes),
            MaxMinutesRoute = routeList.Max(r => (int)(r.DestinationDateTime - r.OriginDateTime).TotalMinutes)
        };
    }
}
