using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/Swashbuckle/OpenApi services
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(opt => {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        opt.SwaggerDoc("v1", new() { Title = assemblyName, Version = "v1" });
        // Use generated XML code documentation
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml"));
    });

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("Example", () => new HealthCheckResult(HealthStatus.Healthy), ["Debug"]);

var app = builder.Build();

app
    .UseHealthChecks("/health")
    .UseSwagger()
    .UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();
