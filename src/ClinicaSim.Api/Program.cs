using ClinicaSim.Infrastructure.DependencyInjection;
using ClinicaSim.Infrastructure.Persistence;
using ClinicaSim.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicyName = "CorsPolicy";

var connectionString = builder.Configuration.GetConnectionString("ClinicaSim")
    ?? throw new InvalidOperationException("Connection string 'ClinicaSim' was not found.");

builder.Services.AddDbContext<ClinicaSimDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddInfrastructureServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1) services (antes do builder.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicaSimDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(dbContext);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);

app.UseCors("DevCors");

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ClinicaSim.Api"
}));



app.Run();
