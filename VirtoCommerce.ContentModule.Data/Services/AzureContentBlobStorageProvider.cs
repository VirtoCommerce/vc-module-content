﻿using System;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Azure;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class AzureContentBlobStorageProvider : AzureBlobProvider, IContentBlobStorageProvider
    {
        private readonly string _chrootPath;
        public AzureContentBlobStorageProvider(string connectionString, string chrootPath)
            : base(connectionString)
        {
            if (chrootPath == null)
                throw new ArgumentNullException(nameof(chrootPath));

            chrootPath = chrootPath.Replace('/', '\\');
            _chrootPath = "\\" + chrootPath.TrimStart('\\');
        }

        #region IContentStorageProvider Members
        public void MoveContent(string srcUrl, string destUrl)
        {
            base.Move(srcUrl, destUrl);
        }

        public void CopyContent(string srcUrl, string destUrl)
        {
            base.Copy(srcUrl, destUrl);
        }
        #endregion
        public override Stream OpenRead(string url)
        {
            return base.OpenRead(NormalizeUrl(url));
        }

        public override void CreateFolder(BlobFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (folder.ParentUrl.IsNullOrEmpty())
            {
                folder.Name = NormalizeUrl(folder.Name);
            }
            base.CreateFolder(folder);
        }

        public override Stream OpenWrite(string url)
        {
            return base.OpenWrite(NormalizeUrl(url));
        }

        public override void Remove(string[] urls)
        {
            urls = urls.Select(NormalizeUrl).ToArray();

            base.Remove(urls);
        }
        public override BlobSearchResult Search(string folderUrl, string keyword)
        {
            folderUrl = NormalizeUrl(folderUrl);
            return base.Search(folderUrl, keyword);
        }

        /// <summary>
        /// Chroot url (artificial add parent 'chroot' folder)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string NormalizeUrl(string url)
        {
            var retVal = _chrootPath;
            if (!string.IsNullOrEmpty(url))
            {
                if (url.IsAbsoluteUrl())
                {
                    url = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
                }
                retVal = "\\" + url.Replace('/', '\\').TrimStart('\\');
                retVal = _chrootPath + "\\" + retVal.Replace(_chrootPath, string.Empty);
                retVal = retVal.Replace("\\\\", "\\");
            }
            return retVal;
        }
    }
}
