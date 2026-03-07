using ClinicaSim.Application.Interfaces;
using ClinicaSim.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaSim.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICasesService, CasesService>();
        services.AddScoped<IAdminCaseService, AdminCaseService>();
        services.AddScoped<IAdminQuestionService, AdminQuestionService>();
        services.AddScoped<IAdminFindingService, AdminFindingService>();
        services.AddScoped<IQuestionBankService, QuestionBankService>();
        services.AddScoped<IFindingBankService, FindingBankService>();
        services.AddScoped<ICaseMappingService, CaseMappingService>();
        services.AddScoped<ISessionsService, SessionsService>();
        services.AddScoped<IPdfReportService, PdfReportService>();
        return services;
    }
}
