using System;
using VirtoCommerce.FileSystemAssetsModule.Core;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobOptions : FileSystemBlobOptions, ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
