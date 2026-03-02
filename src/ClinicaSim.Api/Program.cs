using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "CorsPolicy";

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

app.UseCors(corsPolicyName);

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ClinicaSim.Api"
}));

if (app.Environment.IsDevelopment())
{
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
            paths = new
            {
                ["/health"] = new
                {
                    get = new
                    {
                        tags = new[] { "Health" },
                        responses = new
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

app.Run();
