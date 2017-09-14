using System;
using Omu.ValueInjecter;
using VirtoCommerce.ContentModule.Data.Models;
using VirtoCommerce.Platform.Data.Common.ConventionInjections;

namespace VirtoCommerce.ContentModule.Data.Converters
{
    public static class MenuLinkConverter
    {
        /// <summary>
        /// Patch changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Patch(this MenuLink source, MenuLink target)
        {
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (target == null)
                throw new ArgumentNullException("target");

			var patchInjectionPolicy = new PatchInjection<MenuLink>(x => x.Priority, x => x.Title, x => x.Url);

            target.AssociatedObjectId = source.AssociatedObjectId;
            target.AssociatedObjectName = source.AssociatedObjectName;
            target.AssociatedObjectType = source.AssociatedObjectType;

            target.InjectFrom(patchInjectionPolicy, source);
        }
    }
}
