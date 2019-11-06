using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Data.Handlers
{
    public class IndexContentChangedEventHandler : IEventHandler<ContentChangedEvent>
    {
        public Task Handle(ContentChangedEvent message)
        {
            throw new System.NotImplementedException();
        }
    }
}
