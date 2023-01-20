using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentBuilder : IIndexDocumentBuilder
    {
        private readonly IContentService _contentService;
        private readonly ISearchService<StoreSearchCriteria, StoreSearchResult, Store> _storeService;
        private readonly IContentItemTypeRegistrar _contentItemTypeRegistrar;

        public ContentIndexDocumentBuilder(
            IContentService contentService,
            IStoreSearchService storeService,
            IContentItemTypeRegistrar contentItemTypeRegistrar
        )
        {
            _contentService = contentService;
            _storeService = (ISearchService<StoreSearchCriteria, StoreSearchResult, Store>)storeService;
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
                    var document = await CreateDocument(pair.Value, pair.Key);
                    if (document != null)
                    {
                        result.Add(document);
                    }
                }
                catch (Exception e)
                {
                    // todo: how to log?
                }
            }

            return result;
        }

        private async Task<IndexDocument> CreateDocument(ContentFile file, string storeId)
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
                var parts = id.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    var file = await _contentService.GetFileAsync(ContentConstants.ContentTypes.Pages, parts[0], parts[1]);
                    if (file != null)
                    {
                        result.Add(new KeyValuePair<string, ContentFile>(parts[0], file));
                    }
                }
            }
            return result;
        }
    }
}
