using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class IndexableContentFile : ContentFile, IEntity
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
        public string Permalink { get; set; }
        public string DisplayName { get; set; }
    }
}
