using System;
using System.IO;
using System.Linq;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

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

            var index = relativeUrl.LastIndexOf('/');
            if (index == -1)
            {
                return "/";
            }
            var result = relativeUrl.Substring(0, index);
            if (result.IsNullOrEmpty())
            {
                return "/";
            }

            return result;
        }

        public static string GetLanguage(this string relativeUrl)
        {
            var parts = Path.GetFileName(relativeUrl)?.Split('.');

            if (parts?.Length >= 3)
            {
                return parts[^2];
            }

            return null;
        }

        public static string GetFileNameWithoutLanguage(this string relativeUrl)
        {
            var parts = Path.GetFileName(relativeUrl)?.Split('.');
            if (parts == null || parts.Length == 0)
            {
                return string.Empty;
            }
            if (parts.Length <= 3)
            {
                return parts[0];
            }
            return string.Join('.', parts.Take(parts.Length - 2));
        }

        public static GenericChangedEntry<ContentFile> CreateChangedEntry(string oldUrl, string newUrl, EntryState state = EntryState.Modified)
        {
            ContentFile oldEntry = null;
            ContentFile newEntry = null;

            if (oldUrl != null)
            {
                oldEntry = AbstractTypeFactory<ContentFile>.TryCreateInstance();
                oldEntry.RelativeUrl = oldUrl;
            }

            if (newUrl != null)
            {
                newEntry = AbstractTypeFactory<ContentFile>.TryCreateInstance();
                newEntry.RelativeUrl = newUrl;
            }

            return new GenericChangedEntry<ContentFile>(newEntry, oldEntry, state);
        }
    }
}
