using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentBuilder : IIndexDocumentBuilder
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

        public virtual async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var result = new List<IndexDocument>();

            foreach (var documentId in documentIds)
            {
                try
                {
                    var (storeId, file) = await GetFile(documentId);
                    if (file != null)
                    {
                        var document = await CreateDocument(storeId, file);
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
            }

            return result;
        }

        private async Task<(string, ContentFile)> GetFile(string documentId)
        {
            var (storeId, relativeUrl) = DocumentIdentifierHelper.ParseId(documentId);
            if (storeId != null)
            {
                var file = await _contentService.GetFileAsync(ContentConstants.ContentTypes.Pages, storeId, relativeUrl);
                if (file != null)
                {
                    return (storeId, file);
                }
            }
            return (null, null);
        }

        private async Task<IndexDocument> CreateDocument(string storeId, ContentFile file)
        {
            IndexDocument result = null;
            var fileType = Path.GetExtension(file.RelativeUrl);
            var builder = _contentItemTypeRegistrar.GetContentItemBuilderByType(fileType);
            if (builder != null)
            {
                var indexableFile = await _contentService.GetFileContentAsync(ContentConstants.ContentTypes.Pages, storeId, file.RelativeUrl);
                result = builder.BuildIndexDocument(storeId, indexableFile);
            }
            return result;
        }
    }
}
