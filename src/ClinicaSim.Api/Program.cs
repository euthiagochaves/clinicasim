using System.Text.Json;
using ClinicaSim.Infrastructure.Persistence;
using ClinicaSim.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "CorsPolicy";

var connectionString = builder.Configuration.GetConnectionString("ClinicaSim")
    ?? throw new InvalidOperationException("Connection string 'ClinicaSim' was not found.");

builder.Services.AddDbContext<ClinicaSimDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicaSimDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(dbContext);

    app.MapGet("/swagger/v1/swagger.json", (HttpContext context) =>
    {
        var serverUrl = $"{context.Request.Scheme}://{context.Request.Host.Value}";

        var openApiDocument = new
        {
            openapi = "3.0.1",
            info = new
            {
                title = "ClinicaSim.Api",
                version = "v1"
            },
            servers = new[]
            {
                new { url = serverUrl }
            },
            paths = new Dictionary<string, object>
            {
                ["/health"] = new
                {
                    get = new
                    {
                        tags = new[] { "Health" },
                        responses = new Dictionary<string, object>
                        {
                            ["200"] = new
                            {
                                description = "OK"
                            }
                        }
                    }
                }
            }
        };

        return Results.Json(openApiDocument, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    });

    app.MapGet("/swagger", () => Results.Content(
        """
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>ClinicaSim.Api - Swagger UI</title>
          <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
          <script>
            window.ui = SwaggerUIBundle({
              url: '/swagger/v1/swagger.json',
              dom_id: '#swagger-ui'
            });
          </script>
        </body>
        </html>
        """,
        "text/html"
    ));
}

app.UseCors(corsPolicyName);

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ClinicaSim.Api"
}));

app.Run();
