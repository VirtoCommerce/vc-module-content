using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Microsoft.Practices.Unity;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Web.ExportImport;
using VirtoCommerce.ContentModule.Web.Security;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Assets;
using VirtoCommerce.Platform.Data.Azure;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.ContentModule.Web
{
    public class Module : ModuleBase, ISupportExportImportModule
    {
        private const string _connectionStringName = "VirtoCommerce";
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }


        #region Public Methods and Operators

        public override void Initialize()
        {
            Func<IMenuRepository> menuRepFactory = () =>
                new ContentRepositoryImpl(_connectionStringName, _container.Resolve<AuditableInterceptor>(), new EntityPrimaryKeyGeneratorInterceptor());

            _container.RegisterInstance(menuRepFactory);
            _container.RegisterType<IMenuService, MenuServiceImpl>();

            var settingManager = _container.Resolve<ISettingsManager>();         

            Func<string, IContentBlobStorageProvider> contentProviderFactory = chrootPath =>
           {
               var connectionString = settingManager.GetValue("VirtoCommerce.Content.CmsContentConnectionString", string.Empty);
               var configConnectionString = ConfigurationManager.ConnectionStrings["CmsContentConnectionString"];
               if (configConnectionString != null && !string.IsNullOrEmpty(configConnectionString.ConnectionString))
               {
                   connectionString = configConnectionString.ConnectionString;
               }              

               if (string.IsNullOrEmpty(connectionString))
               {
                   throw new InvalidOperationException("CmsContentConnectionString not defined. Please define module setting VirtoCommerce.Content.CmsContentConnectionString or in web.config");
               }
               var blobConnectionString = BlobConnectionString.Parse(connectionString);
               if (string.Equals(blobConnectionString.Provider, FileSystemBlobProvider.ProviderName, StringComparison.OrdinalIgnoreCase))
               {
                   var storagePath = Path.Combine(NormalizePath(blobConnectionString.RootPath), chrootPath.Replace("/", "\\"));
                   //Use content api/content as public url by default             
                   var publicUrl = VirtualPathUtility.ToAbsolute("~/api/content/" + chrootPath) + "?relativeUrl=";
                   if(!string.IsNullOrEmpty(blobConnectionString.PublicUrl))
                   {
                       publicUrl = blobConnectionString.PublicUrl + "/" + chrootPath;
                   }
                   //Do not export default theme (Themes/default) its will distributed with code
                   return new FileSystemContentBlobStorageProvider(storagePath, publicUrl, "/Themes/default");
               }
               else if (string.Equals(blobConnectionString.Provider, AzureBlobProvider.ProviderName, StringComparison.OrdinalIgnoreCase))
               {
                   return new AzureContentBlobStorageProvider(blobConnectionString.ConnectionString, Path.Combine(blobConnectionString.RootPath, chrootPath));
               }
               throw new InvalidOperationException("Unknown storage provider: " + blobConnectionString.Provider);
           };
            _container.RegisterInstance(contentProviderFactory);
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            
            var dynamicPropertyService = _container.Resolve<IDynamicPropertyService>();

            //https://jekyllrb.com/docs/frontmatter/
            //Register special ContentItem.FrontMatterHeaders type which will be used to define YAML headers for pages, blogs and posts
            var frontMatterHeaderType = "VirtoCommerce.ContentModule.Web.FrontMatterHeaders";
            dynamicPropertyService.RegisterType(frontMatterHeaderType);
            //Title
            var titleHeader = new DynamicProperty
            {
                Id = "Title_FrontMatterHeader",
                Name = "title",
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };

            //If set, this specifies the layout file to use. Use the layout file name without the file extension. 
            var layoutHeader = new DynamicProperty
            {
                Id = "Layout_FrontMatterHeader",
                Name = "layout",
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };
            //If you need your processed blog post URLs to be something other than the site-wide style (default /year/month/day/title.html), then you can set this variable and it will be used as the final URL.
            var permalinkHeader = new DynamicProperty
            {
                Id = "Permalink_FrontMatterHeader",
                Name = "permalink",
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };
            //Set to false if you don’t want a specific post to show up when the site is generated.
            var publishedHeader = new DynamicProperty
            {
                Id = "Published_FrontMatterHeader",
                Name = "published",
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };
            //Instead of placing posts inside of folders, you can specify one or more categories that the post belongs to. When the site is generated the post will act as though it had been set with these categories normally. Categories (plural key) can be specified as a YAML list or a comma-separated string.
            var categoryHeader = new DynamicProperty
            {
                Id = "Category_FrontMatterHeader",
                Name = "category",
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };
            var categoriesHeader = new DynamicProperty
            {
                Id = "Categories_FrontMatterHeader",
                Name = "categories",
                IsArray = true,
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };
            //Similar to categories, one or multiple tags can be added to a post. Also like categories, tags can be specified as a YAML list or a comma-separated string.
            var tagsHeader = new DynamicProperty
            {
                Id = "Tags_FrontMatterHeader",
                Name = "tags",
                IsArray = true,
                ObjectType = frontMatterHeaderType,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };


            //Create DefaultTheme dynamic property for  Store 
            var defaultThemeNameProperty = new DynamicProperty
            {
                Id = "Default_Theme_Name_Property",
                Name = "DefaultThemeName",
                ObjectType = typeof(Store).FullName,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto"
            };

            dynamicPropertyService.SaveProperties(new[] { titleHeader, defaultThemeNameProperty, permalinkHeader, layoutHeader, publishedHeader, categoryHeader, categoriesHeader, tagsHeader });

            //Register bounded security scope types
            var securityScopeService = _container.Resolve<IPermissionScopeService>();
            securityScopeService.RegisterSope(() => new ContentSelectedStoreScope());
        }

        public override void SetupDatabase()
        {
            base.SetupDatabase();

            using (var context = new ContentRepositoryImpl(_connectionStringName, _container.Resolve<AuditableInterceptor>()))
            {
                var initializer = new SetupDatabaseInitializer<ContentRepositoryImpl, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }
        }

        #endregion

        #region ISupportExportImportModule Members

        public void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var exportJob = _container.Resolve<ContentExportImport>();
            exportJob.DoExport(outStream, manifest, progressCallback);
        }

        public void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var exportJob = _container.Resolve<ContentExportImport>();
            exportJob.DoImport(inputStream, manifest, progressCallback);
        }

        public string ExportDescription
        {
            get
            {
                var settingManager = _container.Resolve<ISettingsManager>();
                return settingManager.GetValue("VirtoCommerce.Content.ExportImport.Description", string.Empty);
            }
        }

        #endregion


        private string NormalizePath(string path)
        {
            string result;

            if (path.StartsWith("~"))
            {
                result = HostingEnvironment.MapPath(path);
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }
            else
            {
                result = HostingEnvironment.MapPath("~/");
                result += path;
            }

            return result != null ? Path.GetFullPath(result) : null;
        }
    }
}
