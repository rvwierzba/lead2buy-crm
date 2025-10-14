using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lead2Buy.API.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // ⚠️ Ajuste a connection string conforme seu ambiente Docker/Postgres
            // Se você usa docker-compose, provavelmente é algo como:
            // "Host=postgres;Database=lead2buy;Username=postgres;Password=postgres"
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=lead2buy_db;Username=postgres;Password=@RVWSecretLeadsData");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}