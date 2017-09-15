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
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (target == null)
                throw new ArgumentNullException("target");

            target.Language = source.Language;
            target.Name = source.Name;            

            if (!source.MenuLinks.IsNullCollection())
            {
                source.MenuLinks.Patch(target.MenuLinks, (sourceLink, targetLink) => sourceLink.Patch(targetLink));
            }
        }
    }
}
