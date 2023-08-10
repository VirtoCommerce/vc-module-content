using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.AzureBlobAssetsModule.Core;
using VirtoCommerce.ContentModule.Azure;
using VirtoCommerce.ContentModule.Azure.Extensions;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.ExportImport;
using VirtoCommerce.ContentModule.Data.Handlers;
using VirtoCommerce.ContentModule.Data.MySql;
using VirtoCommerce.ContentModule.Data.PostgreSql;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Data.SqlServer;
using VirtoCommerce.ContentModule.FileSystem;
using VirtoCommerce.ContentModule.FileSystem.Extensions;
using VirtoCommerce.ContentModule.Web.Extensions;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport, IHasConfiguration
    {
        private IApplicationBuilder _appBuilder;

        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {

            serviceCollection.AddTransient<LogChangesChangedEventHandler>();

            serviceCollection.AddDbContext<MenuDbContext>(options =>
            {
                var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });

            serviceCollection.AddTransient<IMenuRepository, MenuRepository>();
            serviceCollection.AddTransient<Func<IMenuRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IMenuRepository>());

            serviceCollection.AddTransient<IMenuService, MenuService>();
            serviceCollection.AddTransient<IMenuLinkListService, MenuLinkListService>();
            serviceCollection.AddTransient<IMenuLinkListSearchService, MenuLinkListSearchService>();
            serviceCollection.AddTransient<IFullTextContentSearchService, FullTextContentSearchService>();
            serviceCollection.AddTransient<IContentService, ContentService>();
            serviceCollection.AddTransient<IContentStatisticService, ContentStatisticService>();
            serviceCollection.AddTransient<IContentFileService, ContentFileService>();
            serviceCollection.AddTransient<IContentPathResolver, ContentPathResolver>();

            serviceCollection.AddSingleton<IContentItemTypeRegistrar, ContentItemTypeRegistrar>();

            var isFullTextSearchEnabled = Configuration.IsContentFullTextSearchEnabled();

            if (isFullTextSearchEnabled)
            {
                serviceCollection.AddTransient<ContentSearchRequestBuilder>();
                serviceCollection.AddTransient<MarkdownContentItemBuilder>();

                serviceCollection.AddTransient<ContentIndexDocumentChangesProvider>();
                serviceCollection.AddTransient<ContentIndexDocumentBuilder>();

                serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
                {
                    DocumentType = FullTextContentSearchService.ContentDocumentType,
                    DocumentSource = new IndexDocumentSource
                    {
                        ChangesProvider = provider.GetService<ContentIndexDocumentChangesProvider>(),
                        DocumentBuilder = provider.GetService<ContentIndexDocumentBuilder>(),
                    },
                });
            }

            serviceCollection.AddTransient<ContentExportImport>();

            serviceCollection.AddOptions<ContentOptions>().Bind(Configuration.GetSection("Content"));

            var contentProvider = Configuration.GetSection("Content:Provider").Value;
            if (contentProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                serviceCollection.AddOptions<AzureContentBlobOptions>().Bind(Configuration.GetSection("Content:AzureBlobStorage")).ValidateDataAnnotations();
                serviceCollection.AddAzureContentBlobProvider();
            }
            else
            {
                serviceCollection.AddOptions<FileSystemContentBlobOptions>().Bind(Configuration.GetSection("Content:FileSystem"))
                    .PostConfigure<IWebHostEnvironment>((options, env) =>
                    {
                        options.RootPath = env.MapPath(options.RootPath);
                    })
                    .ValidateDataAnnotations();

                serviceCollection.AddFileSystemContentBlobProvider();
            }
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<FrontMatterHeaders>();

            //Register module permissions
            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "Content", ContentConstants.Security.Permissions.AllPermissions);

            //Events handlers registration
            var handlerRegistrar = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            handlerRegistrar.RegisterHandler<MenuLinkListChangedEvent>(async (message, _) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var menuDbContext = serviceScope.ServiceProvider.GetRequiredService<MenuDbContext>())
                {
                    var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                    if (databaseProvider == "SqlServer")
                    {
                        menuDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                    }

                    menuDbContext.Database.Migrate();
                }
            }

            var dynamicPropertyService = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyService>();
            dynamicPropertyService.SaveDynamicPropertiesAsync(FrontMatterHeaders.AllDynamicProperties.ToArray()).GetAwaiter().GetResult();

            var isFullTextSearchEnabled = Configuration.IsContentFullTextSearchEnabled();

            if (isFullTextSearchEnabled)
            {
                var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
                settingsRegistrar.RegisterSettings(ContentConstants.Settings.AllSettings, ModuleInfo.Id);

                var searchRequestBuilderRegistrar = appBuilder.ApplicationServices.GetService<ISearchRequestBuilderRegistrar>();
                searchRequestBuilderRegistrar.Register(FullTextContentSearchService.ContentDocumentType, appBuilder.ApplicationServices.GetService<ContentSearchRequestBuilder>);

                var contentItemTypeRegistrar = appBuilder.ApplicationServices.GetService<IContentItemTypeRegistrar>();
                contentItemTypeRegistrar.RegisterContentItemType(".md", appBuilder.ApplicationServices.GetService<MarkdownContentItemBuilder>);
            }
        }

        public void Uninstall()
        {
            // Nothing to do here
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().DoExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().DoImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
