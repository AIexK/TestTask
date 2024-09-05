using TestTask.Api.Middlewares;

namespace TestTask.Api;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplication
            .CreateBuilder(args)
            .RegisterServices()
            .Build()
            .SetupMiddleware()
            .Run();
    }
}
