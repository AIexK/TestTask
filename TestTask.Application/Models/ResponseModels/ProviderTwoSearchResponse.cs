using TestTask.Application.Models.Common;

namespace TestTask.Application.Models.ResponseModels;

public class ProviderTwoSearchResponse
{
    // Mandatory
    // Array of routes
    public ProviderTwoRoute[] Routes { get; set; }
}
