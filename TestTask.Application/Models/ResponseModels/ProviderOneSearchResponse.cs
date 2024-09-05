using TestTask.Application.Models.Common;

namespace TestTask.Application.Models.ResponseModels;

public class ProviderOneSearchResponse
{
    // Mandatory
    // Array of routes
    public ProviderOneRoute[] Routes { get; set; }
}