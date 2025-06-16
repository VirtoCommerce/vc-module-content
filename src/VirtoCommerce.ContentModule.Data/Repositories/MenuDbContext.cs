using System.Reflection;
using EntityFrameworkCore.Triggers;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class MenuDbContext : DbContextBase
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options)
            : base(options)
        {
        }

        protected MenuDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region MenuLinkList

            modelBuilder.Entity<MenuLinkListEntity>().ToTable("ContentMenuLinkList").HasKey(x => x.Id);
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region MenuLink

            modelBuilder.Entity<MenuLinkEntity>().ToTable("ContentMenuLink").HasKey(x => x.Id);
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            modelBuilder.Entity<MenuLinkEntity>()
                .HasOne(m => m.MenuLinkList)
                .WithMany(m => m.MenuLinks)
                .HasForeignKey(m => m.MenuLinkListId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region MenuItem

            modelBuilder.Entity<MenuItemEntity>().ToTable("ContentMenuItem").HasKey(x => x.Id);
            modelBuilder.Entity<MenuItemEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<MenuItemEntity>().HasOne(x => x.ParentMenuItem).WithMany(x => x.Items)
                        .HasForeignKey(x => x.ParentMenuItemId).OnDelete(DeleteBehavior.NoAction);

            #endregion

            base.OnModelCreating(modelBuilder);

            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.CoreModule.Data.XXX project. /> 
            switch (this.Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ContentModule.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ContentModule.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.ContentModule.Data.SqlServer"));
                    break;
            }

        }
    }
}
