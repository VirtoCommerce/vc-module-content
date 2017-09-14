using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using VirtoCommerce.ContentModule.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class ContentRepositoryImpl : EFRepositoryBase, IMenuRepository
    {
        public ContentRepositoryImpl()
        {
        }

        public ContentRepositoryImpl(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
			if (modelBuilder == null)
				throw new ArgumentNullException(nameof(modelBuilder));

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<MenuLinkList>().HasKey(x => x.Id)
                        .Property(x => x.Id);

            modelBuilder.Entity<MenuLinkList>().ToTable("ContentMenuLinkList");

            modelBuilder.Entity<MenuLink>().HasKey(x => x.Id)
                        .Property(x => x.Id);

            modelBuilder.Entity<MenuLink>().HasOptional(m => m.MenuLinkList)
                        .WithMany(m => m.MenuLinks)
                        .HasForeignKey(m => m.MenuLinkListId).WillCascadeOnDelete(true);

            modelBuilder.Entity<MenuLink>().ToTable("ContentMenuLink");

        }

        public IQueryable<MenuLinkList> MenuLinkLists
        {
            get { return GetAsQueryable<MenuLinkList>(); }
        }

        public IQueryable<MenuLink> MenuLinks
        {
            get { return GetAsQueryable<MenuLink>(); }
        }

        public IEnumerable<MenuLinkList> GetAllLinkLists()
        {
            return MenuLinkLists.Include(s => s.MenuLinks).ToArray();
        }

        public IEnumerable<MenuLinkList> GetListsByStoreId(string storeId)
        {
            return MenuLinkLists.Include(s => s.MenuLinks).Where(s => s.StoreId == storeId);
        }

        public MenuLinkList GetListById(string listId)
        {
            return MenuLinkLists.Include(s => s.MenuLinks).FirstOrDefault(s => s.Id == listId);
        }
    }
}
