using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class ContentSearchResult : SearchCriteriaBase
    {
        public ContentSearchResult()
        {
            ObjectType = nameof(ContentFile);
        }
    }
}
