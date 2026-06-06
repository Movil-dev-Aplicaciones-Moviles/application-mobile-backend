namespace BackendAwSmartstay.API.Shared.Infrastructure.Documentation.OpenApi.Configuration.Extensions;

/// <summary>
/// Extension methods for configuring OpenAPI and CORS middleware in the application pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures OpenAPI/Swagger middleware for API documentation and testing interface.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <remarks>
    /// Enables Swagger UI at /swagger endpoint for interactive API documentation.
    /// </remarks>
    public static void UseOpenApiConfiguration(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    /// <summary>
    /// Applies the configured CORS policy to allow cross-origin requests.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    public static void UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("AllowAllPolicy");
    }
}