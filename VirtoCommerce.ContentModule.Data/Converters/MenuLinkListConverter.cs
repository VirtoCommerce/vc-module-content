using System;
using Omu.ValueInjecter;
using VirtoCommerce.ContentModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common.ConventionInjections;

namespace VirtoCommerce.ContentModule.Data.Converters
{
    public static class MenuLinkListConverter
    {
        /// <summary>
        /// Patch changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Patch(this MenuLinkList source, MenuLinkList target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var patchInjectionPolicy = new PatchInjection<MenuLinkList>(x => x.Language, x => x.Name);
            target.InjectFrom(patchInjectionPolicy, source);

            if (!source.MenuLinks.IsNullCollection())
            {
                source.MenuLinks.Patch(target.MenuLinks, (sourceLink, targetLink) => sourceLink.Patch(targetLink));
            }
        }
    }
}
