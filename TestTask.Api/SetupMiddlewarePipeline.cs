namespace TestTask.Api;

public static class SetupMiddlewarePipeline
{
    public static WebApplication SetupMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
