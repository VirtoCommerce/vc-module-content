using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.AzureBlobAssetsModule.Core;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobOptions : AzureBlobOptions, ICloneable
    {
        [Required]
        public string RootPath { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
