using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Search.Indexing
{
    public class PagesDocumentBuilder : IIndexDocumentBuilder
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private readonly IContentBlobStorageProvider _storageProvider;

        public PagesDocumentBuilder(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory)
        {
            _contentStorageProviderFactory = contentStorageProviderFactory;
            _storageProvider = contentStorageProviderFactory("Pages");
        }

        public virtual Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var members = GetMarkdownPages(documentIds);

            IList<IndexDocument> result = members.Select(CreateDocument).ToArray();
            return Task.FromResult(result);
        }

        protected virtual IList<BlobInfo> GetMarkdownPages(IList<string> documentIds)
        {
            return documentIds.Where(x => x.ToLower().Trim().EndsWith(".md"))
                .Select(x => _storageProvider.GetBlobInfo(x))
                .ToList();
        }

        protected virtual IndexDocument CreateDocument(BlobInfo blobInfo)
        {
            var document = new IndexDocument(blobInfo.Key);
            string pageStringContent = null;
            using (var stream = _storageProvider.OpenRead(blobInfo.RelativeUrl))
            {
                pageStringContent = stream.ReadToString();
            }

            document.AddFilterableValue("contentType", blobInfo.ContentType);
            document.AddFilterableAndSearchableValue("name", blobInfo.Name);
            document.AddFilterableAndSearchableValue("fileName", blobInfo.FileName);
            document.AddFilterableValue("mimeType", blobInfo.MimeType);
            document.AddFilterableValue("modifiedDate", blobInfo.ModifiedDate);
            document.AddFilterableAndSearchableValue("relativeUrl", blobInfo.RelativeUrl);
            document.AddFilterableValue("size", blobInfo.Size.ToString());
            document.AddFilterableAndSearchableValue("url", blobInfo.Url);
            document.AddFilterableAndSearchableValue("pageContent", pageStringContent);

            return document;
        }
    }
}
