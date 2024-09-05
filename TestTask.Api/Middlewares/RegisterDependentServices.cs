using TestTask.Infrastructure;

namespace TestTask.Api.Middlewares;

public static class RegisterDependentServices
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddInfrastructure(builder.Configuration);
        return builder;
    }
}
