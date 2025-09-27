using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Services;

public class PublishingServices(IContentService contentService) : IPublishingService
{
    public async Task PublishingAsync(string contentType, string storeId, string relativeUrl, bool publish)
    {
        var source = GetRelativeDraftUrl(relativeUrl, publish);
        var target = GetRelativeDraftUrl(relativeUrl, !publish);
        var exists = await contentService.ItemExistsAsync(contentType, storeId, source);
        if (exists)
        {
            if (await contentService.ItemExistsAsync(contentType, storeId, target))
            {
                if (!publish)
                {
                    // the draft cannot be removed if the published version exists
                    // so, just ignore the request
                    return;
                }
                await contentService.DeleteContentAsync(contentType, storeId, [target]);
            }

            await contentService.MoveContentAsync(contentType, storeId, source, target);
        }
    }

    public Task<IEnumerable<ContentFile>> SetFilesStatuses(IEnumerable<ContentFile> files)
    {
        using var enumerator = files.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).GetEnumerator();
        var currentFile = enumerator.Current;
        var result = new List<ContentFile>();
        while (enumerator.MoveNext())
        {
            var nextFile = enumerator.Current;
            if (currentFile != null && nextFile != null)
            {
                if (nextFile.Name == currentFile.Name + "-draft")
                {
                    currentFile.Published = true;
                    currentFile.HasChanges = true;
                    result.Add(currentFile);
                    nextFile = enumerator.MoveNext() ? enumerator.Current : null;
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

    private static void SetFileStatusByName(ContentFile file)
    {
        var isDraft = file.Name.EndsWith("-draft");
        file.HasChanges = isDraft;
        file.Published = !isDraft;

        file.Name = RemoveDraftSuffix(file.Name);
        file.RelativeUrl = RemoveDraftSuffix(file.RelativeUrl);

        string RemoveDraftSuffix(string name) => isDraft
            ? name[..^"-draft".Length]
            : name;
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
        var publishedExists = await contentService.ItemExistsAsync(contentType, storeId, published);
        var draftExists = await contentService.ItemExistsAsync(contentType, storeId, draft);

        var result = new FilePublishStatus
        {
            Published = publishedExists,
            HasChanges = draftExists
        };

        return result;
    }

    public bool IsDraft(string relativeUrl)
    {
        return relativeUrl?.EndsWith("-draft") ?? false;
    }
}
