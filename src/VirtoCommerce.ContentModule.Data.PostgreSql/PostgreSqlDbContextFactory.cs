using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.ContentModule.Data.Repositories;

namespace VirtoCommerce.ContentModule.Data.PostgreSql
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
    {
        public MenuDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MenuDbContext>();
            var connectionString = args.Any() ? args[0] : "User ID = postgres; Password = password; Host = localhost; Port = 5432; Database = virtocommerce3;";

            builder.UseNpgsql(
                connectionString,
                db => db.MigrationsAssembly(typeof(PostgreSqlDataAssemblyMarker).Assembly.GetName().Name));

            return new MenuDbContext(builder.Options);
        }
    }
}
