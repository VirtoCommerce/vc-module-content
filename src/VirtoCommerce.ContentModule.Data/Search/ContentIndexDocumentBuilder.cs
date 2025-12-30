using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentBuilder : IIndexSchemaBuilder, IIndexDocumentBuilder
    {
        private readonly IContentService _contentService;
        private readonly IContentItemTypeRegistrar _contentItemTypeRegistrar;
        private readonly ILogger<ContentIndexDocumentBuilder> _log;

        public ContentIndexDocumentBuilder(
            IContentService contentService,
            ILogger<ContentIndexDocumentBuilder> log,
            IContentItemTypeRegistrar contentItemTypeRegistrar)
        {
            _contentService = contentService;
            _log = log;
            _contentItemTypeRegistrar = contentItemTypeRegistrar;
        }

        public Task BuildSchemaAsync(IndexDocument schema)
        {
            schema.AddFilterableStringAndContentString("StoreId");
            schema.AddFilterableStringAndContentString("OrganizationId");
            schema.AddFilterableStringAndContentString("Name");
            schema.AddFilterableStringAndContentString("CultureName");
            schema.AddFilterableStringAndContentString("ContentType");
            schema.AddFilterableCollectionAndContentString("UserGroups");
            schema.AddFilterableDateTime("StartDate");
            schema.AddFilterableDateTime("EndDate");
            return Task.CompletedTask;
        }

        public virtual async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var result = new ConcurrentBag<IndexDocument>();

            await Parallel.ForEachAsync(documentIds, async (documentId, cancellationToken) =>
            {
                try
                {
                    var (storeId, contentType, file) = await GetFile(documentId);
                    if (file != null)
                    {
                        var document = await CreateDocument(storeId, contentType, file);
                        if (document != null)
                        {
                            result.Add(document);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.LogError(e, "Cannot create document for ID '{DocumentId}'", documentId);
                }
            });

            return result.ToList();
        }

        private async Task<(string, string, ContentFile)> GetFile(string documentId)
        {
            var (storeId, contentType, relativeUrl) = DocumentIdentifierHelper.ParseId(documentId);
            if (storeId != null)
            {
                var file = await _contentService.GetFileAsync(contentType, storeId, relativeUrl);
                if (file != null)
                {
                    return (storeId, contentType, file);
                }
            }
            return (null, null, null);
        }

        private async Task<IndexDocument> CreateDocument(string storeId, string contentType, ContentFile file)
        {
            IndexDocument result = null;
            var fileType = Path.GetExtension(file.RelativeUrl);
            var builder = _contentItemTypeRegistrar.GetContentItemBuilderByType(fileType);
            if (builder != null)
            {
                var indexableFile = await _contentService.GetFileContentAsync(contentType, storeId, file.RelativeUrl);
                if (indexableFile != null)
                {
                    result = builder.BuildIndexDocument(storeId, contentType, indexableFile);
                }
            }
            return result;
        }
    }
}
