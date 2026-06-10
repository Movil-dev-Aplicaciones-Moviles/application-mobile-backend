using BackendAwSmartstay.API.Accommodations.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.Bookings.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.Payments.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.Shared.Infrastructure.Documentation.OpenApi.Configuration.Extensions;
using BackendAwSmartstay.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using BackendAwSmartstay.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.Shared.Infrastructure.Mediator.Cortex.Configuration.Extensions;
using BackendAwSmartstay.API.IAM.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using BackendAwSmartstay.API.IAM.Infrastructure.Extensions;
using BackendAwSmartstay.API.Profiles.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using BackendAwSmartstay.API.shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using BackendAwSmartstay.API.Analytics.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention())
);

// Database
builder.AddDatabaseConfigurationServices();

// OpenAPI / Swagger
builder.AddOpenApiConfigurationServices();

// CORS
builder.AddCorsServices();

// DI / Contextos
builder.AddSharedContextServices();
builder.AddAccommodationsContextServices();
builder.AddBookingsContextServices();
builder.AddPaymentsContextServices();
builder.AddIamContextServices();
builder.AddProfilesContextServices();
builder.AddAnalyticsContextServices();

// Mediator
builder.AddCortexMediatorServices();

//new implementation 
builder.Services.AddHealthChecks()
    .AddMySql(builder.Configuration.GetConnectionString("DefaultConnection")!, 
        name: "mysql-db-check", 
        tags: new[] { "database" });

//redis implementation
var redisOptions = new ConfigurationOptions
{
    EndPoints = { "host.docker.internal:6379" },
    AbortOnConnectFail = false
};

//TODO: Search if it's correct de implementation in the archive  - 1. Configuración del Rate Limiting (P1)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthLimiter", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10; // Máximo 10 intentos por minuto
        opt.QueueLimit = 0;   // Rechazo inmediato sin encolar
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisOptions));
    
var app = builder.Build();

app.EnsureDatabaseCreated();
await app.SeedDatabaseAsync();

// Swagger
app.UseOpenApiConfiguration();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseRequestAuthorization();

app.MapControllers();

// new implementation
app.MapHealthChecks("/health");

app.Run();
