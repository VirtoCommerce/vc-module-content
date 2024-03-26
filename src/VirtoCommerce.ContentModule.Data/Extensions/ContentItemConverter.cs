using System;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Extensions
{
    public static class ContentItemConverter
    {
        public static string RemoveGetParameters(this string url)
        {
            if (url.IsNullOrEmpty())
            {
                return url;
            }

            var index = url.IndexOf('?');
            if (index > 0)
            {
                return url.Substring(0, index);
            }

            return url;
        }

        public static ContentFolder ToContentModel(this BlobFolder blobFolder)
        {
            if (blobFolder == null)
                throw new ArgumentNullException(nameof(blobFolder));

            var contentFolder = AbstractTypeFactory<ContentFolder>.TryCreateInstance();

            contentFolder.Name = blobFolder.Name;
            contentFolder.Url = blobFolder.Url.RemoveGetParameters();
            contentFolder.ParentUrl = blobFolder.ParentUrl.RemoveGetParameters();
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
            to.Url = from.Url.RemoveGetParameters();
            to.Size = from.Size.ToString();
            to.RelativeUrl = from.RelativeUrl;
            to.CreatedDate = from.CreatedDate;
            to.ModifiedDate = from.ModifiedDate;
            to.Type = from.Type;
            to.MimeType = from.ContentType;
        }

        public static string GetParentUrl(this string relativeUrl)
        {
            if (relativeUrl.IsNullOrEmpty())
            {
                return null;
            }

            var result = relativeUrl.Substring(0, relativeUrl.LastIndexOf('/'));
            if (result.IsNullOrEmpty())
            {
                return "/";
            }

            return result;
        }
    }
}
