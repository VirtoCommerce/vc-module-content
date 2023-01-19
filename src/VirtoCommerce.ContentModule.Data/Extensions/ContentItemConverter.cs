using System;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Extensions
{
    public static class ContentItemConverter
    {
        public static ContentFolder ToContentModel(this BlobFolder blobFolder)
        {
            if (blobFolder == null)
                throw new ArgumentNullException(nameof(blobFolder));

            var contentFolder = AbstractTypeFactory<ContentFolder>.TryCreateInstance();

            contentFolder.Name = blobFolder.Name;
            contentFolder.Url = blobFolder.Url;
            contentFolder.ParentUrl = blobFolder.ParentUrl;
            contentFolder.RelativeUrl = blobFolder.RelativeUrl;
            contentFolder.CreatedDate = blobFolder.CreatedDate;
            contentFolder.ModifiedDate = blobFolder.ModifiedDate;
            contentFolder.Type = blobFolder.Type;

            return contentFolder;
        }

        public static ContentFile ToContentModel(this BlobInfo blobInfo)
        {
            if (blobInfo == null)
                throw new ArgumentNullException(nameof(blobInfo));

            var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
            PopulateProperties(blobInfo, contentFile);
            return contentFile;
        }

        public static IndexableContentFile ToIndexableContentModel(this BlobInfo blobInfo)
        {
            if (blobInfo == null)
                throw new ArgumentNullException(nameof(blobInfo));

            var contentFile = AbstractTypeFactory<IndexableContentFile>.TryCreateInstance();
            PopulateProperties(blobInfo, contentFile);
            return contentFile;
        }

        private static void PopulateProperties(BlobInfo from, ContentFile to)
        {
            to.Name = from.Name;
            to.Url = from.Url;
            to.Size = from.Size.ToString();
            to.RelativeUrl = from.RelativeUrl;
            to.CreatedDate = from.CreatedDate;
            to.ModifiedDate = from.ModifiedDate;
            to.Type = from.Type;
            to.MimeType = from.ContentType;
        }
    }
}
