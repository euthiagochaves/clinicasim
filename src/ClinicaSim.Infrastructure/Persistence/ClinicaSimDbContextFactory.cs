using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicaSim.Infrastructure.Persistence;

public class ClinicaSimDbContextFactory : IDesignTimeDbContextFactory<ClinicaSimDbContext>
{
    public ClinicaSimDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClinicaSimDbContext>();

        const string connectionString =
            "Host=localhost;Port=5432;Database=clinicasim_db;Username=clinicasim;Password=clinicasim_pwd;";

        optionsBuilder.UseNpgsql(connectionString);

        return new ClinicaSimDbContext(optionsBuilder.Options);
    }
}
