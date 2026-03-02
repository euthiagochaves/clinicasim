using ClinicaSim.Application.Interfaces;
using ClinicaSim.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaSim.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICasesService, CasesService>();
        services.AddScoped<ISessionsService, SessionsService>();
        return services;
    }
}
