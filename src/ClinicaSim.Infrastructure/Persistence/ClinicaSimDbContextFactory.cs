using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicaSim.Infrastructure.Persistence
{
    public class ClinicaSimDbContextFactory : IDesignTimeDbContextFactory<ClinicaSimDbContext>
    {
        public ClinicaSimDbContext CreateDbContext(string[] args)
        {
            // dotnet-ef roda a partir da pasta onde você executa o comando.
            // Vamos apontar explicitamente para o projeto da API, que contém o appsettings.
            var solutionRoot = Directory.GetCurrentDirectory();

            // Ajuste se sua estrutura for diferente, mas pelo seu comando é essa.
            var apiProjectPath = Path.Combine(solutionRoot, "src", "ClinicaSim.Api");

            if (!Directory.Exists(apiProjectPath))
            {
                // fallback: tenta o diretório atual (caso o comando seja rodado de outro lugar)
                apiProjectPath = solutionRoot;
            }

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            // Pelo seu appsettings, o nome é "ClinicaSim"
            var connectionString = configuration.GetConnectionString("ClinicaSim");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'ClinicaSim' not found in appsettings.");

            var optionsBuilder = new DbContextOptionsBuilder<ClinicaSimDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ClinicaSimDbContext(optionsBuilder.Options);
        }
    }
}