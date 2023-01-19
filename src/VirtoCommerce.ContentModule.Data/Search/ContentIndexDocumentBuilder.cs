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
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentBuilder : IIndexDocumentBuilder
    {
        private readonly IContentService _contentService;
        private readonly ICrudService<Store> _storeService;
        private readonly IContentItemTypeRegistrar _contentItemTypeRegistrar;

        public ContentIndexDocumentBuilder(
            IContentService contentService,
            IStoreService storeService,
            IContentItemTypeRegistrar contentItemTypeRegistrar
        )
        {
            _contentService = contentService;
            _storeService = (ICrudService<Store>)storeService;
            _contentItemTypeRegistrar = contentItemTypeRegistrar;
        }

        public virtual async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var storeId = "B2B-store";
            // todo: get all stores
            var files = await GetFiles(storeId, documentIds);
            var result = new List<IndexDocument>();

            foreach (var file in files)
            {
                var document = await CreateDocument(file, storeId);
                if (document != null)
                {
                    result.Add(document);
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

        private async Task<IEnumerable<ContentFile>> GetFiles(string storeId, IList<string> documentIds)
        {
            var result = new List<ContentFile>();
            foreach (var id in documentIds)
            {
                var file = await _contentService.GetFileAsync(ContentConstants.ContentTypes.Pages, storeId, id);
                if (file != null)
                {
                    result.Add(file);
                }
            }
            return result;            
        }
    }
}
