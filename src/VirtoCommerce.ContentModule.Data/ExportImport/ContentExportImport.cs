using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ContentModule.Data.ExportImport
{
    public sealed class ContentExportImport
    {
        private static readonly string[] _exportedFolders = { "Pages", "Themes" };
        private readonly int _batchSize = 50;

        private readonly IMenuLinkListService _menuLinkListService;
        private readonly IMenuLinkListSearchService _menuLinkListSearchService;
        private readonly IBlobContentStorageProvider _blobContentStorageProvider;
        private readonly JsonSerializer _jsonSerializer;

        public ContentExportImport(
            IMenuLinkListService menuLinkListService,
            IMenuLinkListSearchService menuLinkListSearchService,
            IBlobContentStorageProvider blobContentStorageProvider,
            JsonSerializer jsonSerializer)
        {
            _menuLinkListService = menuLinkListService;
            _menuLinkListSearchService = menuLinkListSearchService;
            _blobContentStorageProvider = blobContentStorageProvider;
            _jsonSerializer = jsonSerializer;
        }

        public async Task DoExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                await writer.WritePropertyNameAsync("MenuLinkLists");
                await writer.SerializeArrayWithPagingAsync(_jsonSerializer, _batchSize,
                    async (skip, take) =>
                        (GenericSearchResult<MenuLinkList>)await _menuLinkListSearchService.SearchNoCloneAsync(new MenuLinkListSearchCriteria
                        {
                            Skip = skip,
                            Take = take
                        })
                    , (processedCount, totalCount) =>
                    {
                        progressInfo.Description = $"{processedCount} of {totalCount} menu link lists have been exported";
                        progressCallback(progressInfo);
                    }, cancellationToken);

                if (options.HandleBinaryData)
                {
                    await writer.WritePropertyNameAsync("CmsContent");
                    await writer.WriteStartArrayAsync();

                    var result = await _blobContentStorageProvider.SearchAsync(string.Empty, null);
                    foreach (var blobFolder in result.Results.Where(x => _exportedFolders.Contains(x.Name)))
                    {
                        var contentFolder = new ContentFolder
                        {
                            Url = blobFolder.RelativeUrl
                        };
                        await ReadContentFoldersRecursiveAsync(contentFolder, progressCallback);

                        _jsonSerializer.Serialize(writer, contentFolder);
                    }

                    await writer.FlushAsync();

                    progressInfo.Description = $"{result.TotalCount} cms content exported";
                    progressCallback(progressInfo);

                    await writer.WriteEndArrayAsync();
                }

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (await reader.ReadAsync())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        var readerValueString = reader.Value?.ToString();

                        if (readerValueString == "MenuLinkLists")
                        {
                            await reader.DeserializeArrayWithPagingAsync<MenuLinkList>(_jsonSerializer, _batchSize,
                            async items =>
                            {
                                await _menuLinkListService.SaveChangesAsync(items);
                            }, processedCount =>
                            {
                                progressInfo.Description = $"{processedCount} menu link lists have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                        }
                        else if (readerValueString == "CmsContent" && options != null && options.HandleBinaryData)
                        {
                            progressInfo.Description = "importing binary data:  themes and pages importing...";
                            progressCallback(progressInfo);

                            await reader.DeserializeArrayWithPagingAsync<ContentFolder>(_jsonSerializer, _batchSize,
                                items =>
                                {
                                    foreach (var item in items)
                                    {
                                        SaveContentFolderRecursive(item, progressCallback);
                                    }
                                    return Task.CompletedTask;
                                }, processedCount =>
                                {
                                    progressInfo.Description = $"{processedCount} menu links have been imported";
                                    progressCallback(progressInfo);
                                }, cancellationToken);
                        }
                    }
                }
            }
        }

        private void SaveContentFolderRecursive(ContentFolder folder, Action<ExportImportProgressInfo> progressCallback)
        {
            foreach (var childFolder in folder.Folders)
            {
                SaveContentFolderRecursive(childFolder, progressCallback);
            }

            foreach (var folderFile in folder.Files)
            {
                using (var stream = _blobContentStorageProvider.OpenWrite(folderFile.Url))
                using (var memStream = new MemoryStream(folderFile.Data))
                {
                    var progressInfo = new ExportImportProgressInfo
                    {
                        Description = $"Saving {folderFile.Url}"
                    };
                    progressCallback(progressInfo);
                    memStream.CopyTo(stream);
                }
            }
        }

        private async Task ReadContentFoldersRecursiveAsync(ContentFolder folder, Action<ExportImportProgressInfo> progressCallback)
        {
            var result = await _blobContentStorageProvider.SearchAsync(folder.Url, null);

            foreach (var blobFolder in result.Results.OfType<BlobFolder>())
            {
                var contentFolder = new ContentFolder
                {
                    Url = blobFolder.RelativeUrl
                };

                await ReadContentFoldersRecursiveAsync(contentFolder, progressCallback);
                folder.Folders.Add(contentFolder);
            }

            foreach (var blobItem in result.Results.OfType<BlobInfo>())
            {
                var progressInfo = new ExportImportProgressInfo
                {
                    Description = $"Read {blobItem.Url}"
                };
                progressCallback(progressInfo);

                var contentFile = new ContentFile
                {
                    Url = blobItem.RelativeUrl
                };
                using (var stream = await _blobContentStorageProvider.OpenReadAsync(blobItem.Url))
                {
                    contentFile.Data = stream.ReadFully();
                }
                folder.Files.Add(contentFile);
            }
        }
    }
}
