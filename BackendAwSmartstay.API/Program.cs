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

var app = builder.Build();

app.EnsureDatabaseCreated();
await app.SeedDatabaseAsync();

// Swagger
app.UseOpenApiConfiguration();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseRequestAuthorization();

app.MapControllers();

// new implementation
app.MapHealthChecks("/health");

app.Run();
