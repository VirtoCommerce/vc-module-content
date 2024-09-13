using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.ContentModule.Data.Repositories;

namespace VirtoCommerce.ContentModule.Data.SqlServer
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
    {
        public MenuDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MenuDbContext>();
            var connectionString = args.Any() ? args[0] : "Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30";

            builder.UseSqlServer(
                connectionString,
                db => db.MigrationsAssembly(typeof(SqlServerDataAssemblyMarker).Assembly.GetName().Name));

            return new MenuDbContext(builder.Options);
        }
    }
}
