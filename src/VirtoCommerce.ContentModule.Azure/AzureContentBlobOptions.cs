using System;
using VirtoCommerce.AzureBlobAssetsModule.Core;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobOptions : AzureBlobOptions, ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
