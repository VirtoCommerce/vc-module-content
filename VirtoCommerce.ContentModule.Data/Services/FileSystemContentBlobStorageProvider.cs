using System;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Data.Assets;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class FileSystemContentBlobStorageProvider : FileSystemBlobProvider, IContentBlobStorageProvider
    {
        public FileSystemContentBlobStorageProvider(string storagePath, string publicUrl)
            : base(storagePath, publicUrl)
        {
        }

        #region IContentStorageProvider Members

        public void MoveContent(string srcUrl, string dstUrl)
        {
            var srcPath = GetStoragePathFromUrl(srcUrl);
            var dstPath = GetStoragePathFromUrl(dstUrl);

            if (srcPath != dstPath)
            {
                if (Directory.Exists(srcPath) && !Directory.Exists(dstPath))
                {
                    Directory.Move(srcPath, dstPath);
                }
                else if (File.Exists(srcPath) && !File.Exists(dstPath))
                {
                    File.Move(srcPath, dstPath);
                }
            }
        }

        public void CopyContent(string srcUrl, string destUrl)
        {
            var srcPath = GetStoragePathFromUrl(srcUrl);
            var destPath = GetStoragePathFromUrl(destUrl);

            CopyDirectoryRecursive(srcPath, destPath);
        }
        #endregion

        public override BlobSearchResult Search(string folderUrl, string keyword)
        {
            if (!string.IsNullOrEmpty(folderUrl))
            {
                return new BlobSearchResult();
            }
            return base.Search(folderUrl, keyword);
        }

        private static void CopyDirectoryRecursive(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = Path.Combine(destPath, Path.GetFileName(folder));
                CopyDirectoryRecursive(folder, dest);
            }
        }
    }
}
