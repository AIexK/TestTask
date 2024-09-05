using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Application.Common.Interfaces;
using TestTask.Infrastructure.Services;

namespace TestTask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }
}
