using webModels = VirtoCommerce.ContentModule.Web.Models;
using coreModels = VirtoCommerce.ContentModule.Data.Models;
using Omu.ValueInjecter;

namespace VirtoCommerce.ContentModule.Web.Converters
{
    public static class MenuLinkConverter
    {
        public static coreModels.MenuLink ToCoreModel(this webModels.MenuLink link)
        {
            var retVal = new coreModels.MenuLink();
            retVal.InjectFrom(link);
            return retVal;
        }

        public static webModels.MenuLink ToWebModel(this coreModels.MenuLink link)
        {
            var retVal = new webModels.MenuLink();
            retVal.InjectFrom(link);
            return retVal;
        }
    }
}