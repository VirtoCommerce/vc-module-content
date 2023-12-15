using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class PublishingServices : IPublishingService
    {
        private readonly IContentService _contentService;

        public PublishingServices(IContentService contentService)
        {
            _contentService = contentService;
        }

        public async Task PublishingAsync(string contentType, string storeId, string relativeUrl, bool publish)
        {
            var source = GetRelativeDraftUrl(relativeUrl, publish);
            var target = GetRelativeDraftUrl(relativeUrl, !publish);
            var exists = await _contentService.ItemExistsAsync(contentType, storeId, source);
            if (exists)
            {
                if (await _contentService.ItemExistsAsync(contentType, storeId, target))
                {
                    if (!publish)
                    {
                        // the draft cannot be removed if the published version exists
                        // so, just ignore the request
                        return;
                    }
                    await _contentService.DeleteContentAsync(contentType, storeId, new[] { target });
                }

                await _contentService.MoveContentAsync(contentType, storeId, source, target);
            }
        }

        public Task<IEnumerable<ContentFile>> SetFilesStatuses(IEnumerable<ContentFile> files)
        {
            using var enumerator = files.OrderBy(x => x.Name).GetEnumerator();
            var currentFile = enumerator.Current;
            var result = new List<ContentFile>();
            while (enumerator.MoveNext())
            {
                var nextFile = enumerator.Current;
                if (currentFile != null && nextFile != null)
                {
                    if (nextFile.Name == currentFile.Name + "-draft")
                    {
                        nextFile.Published = true;
                        nextFile.HasChanges = true;
                        result.Add(nextFile);
                        if (enumerator.MoveNext())
                        {
                            nextFile = enumerator.Current;
                        }
                        else
                        {
                            nextFile = null;
                        }
                    }
                    else
                    {
                        result.Add(currentFile);
                        SetFileStatusByName(currentFile);
                    }
                }
                currentFile = nextFile;
            }

            if (currentFile != null)
            {
                result.Add(currentFile);
                SetFileStatusByName(currentFile);
            }

            return Task.FromResult(result.AsEnumerable());
        }

        private void SetFileStatusByName(ContentFile file)
        {
            var isDraft = file.Name.EndsWith("-draft");
            file.HasChanges = isDraft;
            file.Published = !isDraft;
            file.Name = isDraft
                ? file.Name.Substring(0, file.Name.Length - "-draft".Length)
                : file.Name;
        }

        public string GetRelativeDraftUrl(string source, bool draft)
        {
            if (source.EndsWith("-draft"))
            {
                return draft ? source : source.Substring(0, source.Length - "-draft".Length);
            }

            return draft ? source + "-draft" : source;
        }

        public async Task<FilePublishStatus> PublishStatusAsync(string contentType, string storeId, string relativeUrl)
        {
            var published = GetRelativeDraftUrl(relativeUrl, false);
            var draft = GetRelativeDraftUrl(relativeUrl, true);
            var publishedExists = await _contentService.ItemExistsAsync(contentType, storeId, published);
            var draftExists = await _contentService.ItemExistsAsync(contentType, storeId, draft);

            var result = new FilePublishStatus
            {
                Published = publishedExists,
                HasChanges = draftExists
            };

            return result;
        }
    }
}
