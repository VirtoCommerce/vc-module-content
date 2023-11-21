namespace VirtoCommerce.ContentModule.Core.Model
{
    /// <summary>
    /// Represent content file
    /// </summary>
    public class ContentFile : ContentItem
    {
        public ContentFile()
            : base("blob")
        {
        }

        public string MimeType { get; set; }
        public string Size { get; set; }

        public bool Published { get; set; }
        public bool HasChanges { get; set; }
    }
}
