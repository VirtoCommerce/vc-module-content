using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.AzureBlobAssetsModule.Core;
using VirtoCommerce.ContentModule.Azure;
using VirtoCommerce.ContentModule.Azure.Extensions;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.ExportImport;
using VirtoCommerce.ContentModule.Data.Handlers;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.FileSystem;
using VirtoCommerce.ContentModule.Web.Extensions;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

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

            serviceCollection.AddDbContext<MenuDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });

            serviceCollection.AddTransient<IMenuRepository, MenuRepository>();
            serviceCollection.AddTransient<Func<IMenuRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IMenuRepository>());

            serviceCollection.AddTransient<IMenuService, MenuService>();

            serviceCollection.AddTransient<ContentExportImport>();

            var contentProvider = Configuration.GetSection("Content:Provider").Value;
            if (contentProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                serviceCollection.AddOptions<AzureContentBlobOptions>().Bind(Configuration.GetSection("Content:AzureBlobStorage")).ValidateDataAnnotations();
                serviceCollection.AddAzureContentBlobProvider();
            }
            else
            {
                serviceCollection.AddOptions<FileSystemContentBlobOptions>().Bind(Configuration.GetSection("Content:FileSystem")).ValidateDataAnnotations();

                serviceCollection.AddSingleton<IBlobContentStorageProvider, FileSystemContentBlobStorageProvider>();
                serviceCollection.AddSingleton<IBlobContentStorageProviderFactory, FileSystemContentBlobStorageProviderFactory>((provider) =>
                {
                    var platformOptions = provider.GetService<IOptions<PlatformOptions>>();
                    var settingManager = provider.GetService<ISettingsManager>();
                    var hostingEnvironment = provider.GetService<IWebHostEnvironment>();
                    var fileOptions = provider.GetService<IOptions<FileSystemContentBlobOptions>>();
                    fileOptions.Value.RootPath = hostingEnvironment.MapPath(fileOptions.Value.RootPath);
                    return new FileSystemContentBlobStorageProviderFactory(fileOptions, platformOptions, settingManager);
                });
            }
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<FrontMatterHeaders>();

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ContentConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission
                {
                    GroupName = "Content",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            //Events handlers registration
            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<MenuLinkListChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var menuDbContext = serviceScope.ServiceProvider.GetRequiredService<MenuDbContext>())
                {
                    menuDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                    menuDbContext.Database.EnsureCreated();
                    menuDbContext.Database.Migrate();
                }
            }

            var dynamicPropertyService = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyService>();
            dynamicPropertyService.SaveDynamicPropertiesAsync(FrontMatterHeaders.AllDynamicProperties.ToArray()).GetAwaiter().GetResult();
        }

        public void Uninstall()
        {
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
