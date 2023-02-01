using System;
using System.Collections.Generic;
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
            var files = await GetFiles(documentIds);
            var result = new List<IndexDocument>();

            foreach (var pair in files)
            {
                try
                {
                    var document = await CreateDocument(pair.Key, pair.Value);
                    if (document != null)
                    {
                        result.Add(document);
                    }
                }
                catch (Exception e)
                {
                    _log.LogError(e, "Error while creating document for file {file}", pair.Value.RelativeUrl);
                }
            }

            return result;
        }

        private async Task<IndexDocument> CreateDocument(string storeId, ContentFile file)
        {
            var fileType = System.IO.Path.GetExtension(file.RelativeUrl);
            var builder = _contentItemTypeRegistrar.GetContentItemBuilderByType(fileType);
            if (builder != null)
            {
                var contentFile = await _contentService.GetFileContentAsync(ContentConstants.ContentTypes.Pages, storeId, file.RelativeUrl);
                var result = builder.BuildIndexDocument(storeId, contentFile);
                return result;
            }
            return null;
        }

        private async Task<IEnumerable<KeyValuePair<string, ContentFile>>> GetFiles(IList<string> documentIds)
        {
            var result = new List<KeyValuePair<string, ContentFile>>();
            foreach (var id in documentIds)
            {
                var (storeId, relativeUrl) = DocumentIdentifierHelper.ParseId(id);

                if (storeId != null)
                {
                    var file = await _contentService.GetFileAsync(ContentConstants.ContentTypes.Pages, storeId, relativeUrl);
                    if (file != null)
                    {
                        result.Add(new KeyValuePair<string, ContentFile>(storeId, file));
                    }
                }
            }
            return result;
        }
    }
}
