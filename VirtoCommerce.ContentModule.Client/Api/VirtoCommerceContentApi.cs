using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using VirtoCommerce.ContentModule.Client.Client;
using VirtoCommerce.ContentModule.Client.Model;

namespace VirtoCommerce.ContentModule.Client.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IVirtoCommerceContentApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Copy contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        void ContentCopyContent(string srcPath, string destPath);

        /// <summary>
        /// Copy contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> ContentCopyContentWithHttpInfo(string srcPath, string destPath);
        /// <summary>
        /// Create content folder
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns></returns>
        void ContentCreateContentFolder(string contentType, string storeId, ContentFolder folder);

        /// <summary>
        /// Create content folder
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> ContentCreateContentFolderWithHttpInfo(string contentType, string storeId, ContentFolder folder);
        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns></returns>
        void ContentDeleteContent(string contentType, string storeId, List<string> urls);

        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> ContentDeleteContentWithHttpInfo(string contentType, string storeId, List<string> urls);
        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>byte[]</returns>
        byte[] ContentGetContentItemDataStream(string contentType, string storeId, string relativeUrl);

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>ApiResponse of byte[]</returns>
        ApiResponse<byte[]> ContentGetContentItemDataStreamWithHttpInfo(string contentType, string storeId, string relativeUrl);
        /// <summary>
        /// Return summary content statistic
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>ContentStatistic</returns>
        ContentStatistic ContentGetStoreContentStats(string storeId);

        /// <summary>
        /// Return summary content statistic
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of ContentStatistic</returns>
        ApiResponse<ContentStatistic> ContentGetStoreContentStatsWithHttpInfo(string storeId);
        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns></returns>
        void ContentMoveContent(string contentType, string storeId, string oldUrl, string newUrl);

        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> ContentMoveContentWithHttpInfo(string contentType, string storeId, string oldUrl, string newUrl);
        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>List&lt;ContentItem&gt;</returns>
        List<ContentItem> ContentSearchContent(string contentType, string storeId, string folderUrl = null, string keyword = null);

        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>ApiResponse of List&lt;ContentItem&gt;</returns>
        ApiResponse<List<ContentItem>> ContentSearchContentWithHttpInfo(string contentType, string storeId, string folderUrl = null, string keyword = null);
        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        void ContentUnpack(string contentType, string storeId, string archivePath, string destPath);

        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> ContentUnpackWithHttpInfo(string contentType, string storeId, string archivePath, string destPath);
        /// <summary>
        /// Upload content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>List&lt;ContentItem&gt;</returns>
        List<ContentItem> ContentUploadContent(string contentType, string storeId, string folderUrl, string url = null);

        /// <summary>
        /// Upload content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>ApiResponse of List&lt;ContentItem&gt;</returns>
        ApiResponse<List<ContentItem>> ContentUploadContentWithHttpInfo(string contentType, string storeId, string folderUrl, string url = null);
        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>
        /// Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>bool?</returns>
        bool? MenuCheckName(string storeId, string name, string language = null, string id = null);

        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>
        /// Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>ApiResponse of bool?</returns>
        ApiResponse<bool?> MenuCheckNameWithHttpInfo(string storeId, string name, string language = null, string id = null);
        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        void MenuDelete(List<string> listIds, string storeId);

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> MenuDeleteWithHttpInfo(List<string> listIds, string storeId);
        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>MenuLinkList</returns>
        MenuLinkList MenuGetList(string storeId, string listId);

        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>ApiResponse of MenuLinkList</returns>
        ApiResponse<MenuLinkList> MenuGetListWithHttpInfo(string storeId, string listId);
        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>List&lt;MenuLinkList&gt;</returns>
        List<MenuLinkList> MenuGetLists(string storeId);

        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>ApiResponse of List&lt;MenuLinkList&gt;</returns>
        ApiResponse<List<MenuLinkList>> MenuGetListsWithHttpInfo(string storeId);
        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        void MenuUpdate(MenuLinkList list, string storeId);

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        ApiResponse<Object> MenuUpdateWithHttpInfo(MenuLinkList list, string storeId);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Copy contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ContentCopyContentAsync(string srcPath, string destPath);

        /// <summary>
        /// Copy contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> ContentCopyContentAsyncWithHttpInfo(string srcPath, string destPath);
        /// <summary>
        /// Create content folder
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ContentCreateContentFolderAsync(string contentType, string storeId, ContentFolder folder);

        /// <summary>
        /// Create content folder
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> ContentCreateContentFolderAsyncWithHttpInfo(string contentType, string storeId, ContentFolder folder);
        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ContentDeleteContentAsync(string contentType, string storeId, List<string> urls);

        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> ContentDeleteContentAsyncWithHttpInfo(string contentType, string storeId, List<string> urls);
        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>Task of byte[]</returns>
        System.Threading.Tasks.Task<byte[]> ContentGetContentItemDataStreamAsync(string contentType, string storeId, string relativeUrl);

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>Task of ApiResponse (byte[])</returns>
        System.Threading.Tasks.Task<ApiResponse<byte[]>> ContentGetContentItemDataStreamAsyncWithHttpInfo(string contentType, string storeId, string relativeUrl);
        /// <summary>
        /// Return summary content statistic
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>Task of ContentStatistic</returns>
        System.Threading.Tasks.Task<ContentStatistic> ContentGetStoreContentStatsAsync(string storeId);

        /// <summary>
        /// Return summary content statistic
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse (ContentStatistic)</returns>
        System.Threading.Tasks.Task<ApiResponse<ContentStatistic>> ContentGetStoreContentStatsAsyncWithHttpInfo(string storeId);
        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ContentMoveContentAsync(string contentType, string storeId, string oldUrl, string newUrl);

        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> ContentMoveContentAsyncWithHttpInfo(string contentType, string storeId, string oldUrl, string newUrl);
        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>Task of List&lt;ContentItem&gt;</returns>
        System.Threading.Tasks.Task<List<ContentItem>> ContentSearchContentAsync(string contentType, string storeId, string folderUrl = null, string keyword = null);

        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>Task of ApiResponse (List&lt;ContentItem&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<ContentItem>>> ContentSearchContentAsyncWithHttpInfo(string contentType, string storeId, string folderUrl = null, string keyword = null);
        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task ContentUnpackAsync(string contentType, string storeId, string archivePath, string destPath);

        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> ContentUnpackAsyncWithHttpInfo(string contentType, string storeId, string archivePath, string destPath);
        /// <summary>
        /// Upload content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>Task of List&lt;ContentItem&gt;</returns>
        System.Threading.Tasks.Task<List<ContentItem>> ContentUploadContentAsync(string contentType, string storeId, string folderUrl, string url = null);

        /// <summary>
        /// Upload content item
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>Task of ApiResponse (List&lt;ContentItem&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<ContentItem>>> ContentUploadContentAsyncWithHttpInfo(string contentType, string storeId, string folderUrl, string url = null);
        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>
        /// Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>Task of bool?</returns>
        System.Threading.Tasks.Task<bool?> MenuCheckNameAsync(string storeId, string name, string language = null, string id = null);

        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>
        /// Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>Task of ApiResponse (bool?)</returns>
        System.Threading.Tasks.Task<ApiResponse<bool?>> MenuCheckNameAsyncWithHttpInfo(string storeId, string name, string language = null, string id = null);
        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task MenuDeleteAsync(List<string> listIds, string storeId);

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> MenuDeleteAsyncWithHttpInfo(List<string> listIds, string storeId);
        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>Task of MenuLinkList</returns>
        System.Threading.Tasks.Task<MenuLinkList> MenuGetListAsync(string storeId, string listId);

        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>Task of ApiResponse (MenuLinkList)</returns>
        System.Threading.Tasks.Task<ApiResponse<MenuLinkList>> MenuGetListAsyncWithHttpInfo(string storeId, string listId);
        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of List&lt;MenuLinkList&gt;</returns>
        System.Threading.Tasks.Task<List<MenuLinkList>> MenuGetListsAsync(string storeId);

        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of ApiResponse (List&lt;MenuLinkList&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<MenuLinkList>>> MenuGetListsAsyncWithHttpInfo(string storeId);
        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>Task of void</returns>
        System.Threading.Tasks.Task MenuUpdateAsync(MenuLinkList list, string storeId);

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse</returns>
        System.Threading.Tasks.Task<ApiResponse<object>> MenuUpdateAsyncWithHttpInfo(MenuLinkList list, string storeId);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class VirtoCommerceContentApi : IVirtoCommerceContentApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtoCommerceContentApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="apiClient">An instance of ApiClient.</param>
        /// <returns></returns>
        public VirtoCommerceContentApi(ApiClient apiClient)
        {
            ApiClient = apiClient;
            Configuration = apiClient.Configuration;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the API client object
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient { get; set; }

        /// <summary>
        /// Copy contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        public void ContentCopyContent(string srcPath, string destPath)
        {
             ContentCopyContentWithHttpInfo(srcPath, destPath);
        }

        /// <summary>
        /// Copy contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> ContentCopyContentWithHttpInfo(string srcPath, string destPath)
        {
            // verify the required parameter 'srcPath' is set
            if (srcPath == null)
                throw new ApiException(400, "Missing required parameter 'srcPath' when calling VirtoCommerceContentApi->ContentCopyContent");
            // verify the required parameter 'destPath' is set
            if (destPath == null)
                throw new ApiException(400, "Missing required parameter 'destPath' when calling VirtoCommerceContentApi->ContentCopyContent");

            var localVarPath = "/api/content/copy";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (srcPath != null) localVarQueryParams.Add("srcPath", ApiClient.ParameterToString(srcPath)); // query parameter
            if (destPath != null) localVarQueryParams.Add("destPath", ApiClient.ParameterToString(destPath)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentCopyContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentCopyContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Copy contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task ContentCopyContentAsync(string srcPath, string destPath)
        {
             await ContentCopyContentAsyncWithHttpInfo(srcPath, destPath);

        }

        /// <summary>
        /// Copy contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> ContentCopyContentAsyncWithHttpInfo(string srcPath, string destPath)
        {
            // verify the required parameter 'srcPath' is set
            if (srcPath == null)
                throw new ApiException(400, "Missing required parameter 'srcPath' when calling VirtoCommerceContentApi->ContentCopyContent");
            // verify the required parameter 'destPath' is set
            if (destPath == null)
                throw new ApiException(400, "Missing required parameter 'destPath' when calling VirtoCommerceContentApi->ContentCopyContent");

            var localVarPath = "/api/content/copy";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (srcPath != null) localVarQueryParams.Add("srcPath", ApiClient.ParameterToString(srcPath)); // query parameter
            if (destPath != null) localVarQueryParams.Add("destPath", ApiClient.ParameterToString(destPath)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentCopyContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentCopyContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns></returns>
        public void ContentCreateContentFolder(string contentType, string storeId, ContentFolder folder)
        {
             ContentCreateContentFolderWithHttpInfo(contentType, storeId, folder);
        }

        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> ContentCreateContentFolderWithHttpInfo(string contentType, string storeId, ContentFolder folder)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentCreateContentFolder");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentCreateContentFolder");
            // verify the required parameter 'folder' is set
            if (folder == null)
                throw new ApiException(400, "Missing required parameter 'folder' when calling VirtoCommerceContentApi->ContentCreateContentFolder");

            var localVarPath = "/api/content/{contentType}/{storeId}/folder";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folder.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(folder); // http body (model) parameter
            }
            else
            {
                localVarPostBody = folder; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentCreateContentFolder: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentCreateContentFolder: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task ContentCreateContentFolderAsync(string contentType, string storeId, ContentFolder folder)
        {
             await ContentCreateContentFolderAsyncWithHttpInfo(contentType, storeId, folder);

        }

        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> ContentCreateContentFolderAsyncWithHttpInfo(string contentType, string storeId, ContentFolder folder)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentCreateContentFolder");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentCreateContentFolder");
            // verify the required parameter 'folder' is set
            if (folder == null)
                throw new ApiException(400, "Missing required parameter 'folder' when calling VirtoCommerceContentApi->ContentCreateContentFolder");

            var localVarPath = "/api/content/{contentType}/{storeId}/folder";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folder.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(folder); // http body (model) parameter
            }
            else
            {
                localVarPostBody = folder; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentCreateContentFolder: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentCreateContentFolder: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Delete content from server 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns></returns>
        public void ContentDeleteContent(string contentType, string storeId, List<string> urls)
        {
             ContentDeleteContentWithHttpInfo(contentType, storeId, urls);
        }

        /// <summary>
        /// Delete content from server 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> ContentDeleteContentWithHttpInfo(string contentType, string storeId, List<string> urls)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentDeleteContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentDeleteContent");
            // verify the required parameter 'urls' is set
            if (urls == null)
                throw new ApiException(400, "Missing required parameter 'urls' when calling VirtoCommerceContentApi->ContentDeleteContent");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (urls != null) localVarQueryParams.Add("urls", ApiClient.ParameterToString(urls)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentDeleteContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentDeleteContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Delete content from server 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task ContentDeleteContentAsync(string contentType, string storeId, List<string> urls)
        {
             await ContentDeleteContentAsyncWithHttpInfo(contentType, storeId, urls);

        }

        /// <summary>
        /// Delete content from server 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> ContentDeleteContentAsyncWithHttpInfo(string contentType, string storeId, List<string> urls)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentDeleteContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentDeleteContent");
            // verify the required parameter 'urls' is set
            if (urls == null)
                throw new ApiException(400, "Missing required parameter 'urls' when calling VirtoCommerceContentApi->ContentDeleteContent");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (urls != null) localVarQueryParams.Add("urls", ApiClient.ParameterToString(urls)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentDeleteContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentDeleteContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager) 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>byte[]</returns>
        public byte[] ContentGetContentItemDataStream(string contentType, string storeId, string relativeUrl)
        {
             ApiResponse<byte[]> localVarResponse = ContentGetContentItemDataStreamWithHttpInfo(contentType, storeId, relativeUrl);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager) 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>ApiResponse of byte[]</returns>
        public ApiResponse<byte[]> ContentGetContentItemDataStreamWithHttpInfo(string contentType, string storeId, string relativeUrl)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");
            // verify the required parameter 'relativeUrl' is set
            if (relativeUrl == null)
                throw new ApiException(400, "Missing required parameter 'relativeUrl' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (relativeUrl != null) localVarQueryParams.Add("relativeUrl", ApiClient.ParameterToString(relativeUrl)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentGetContentItemDataStream: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentGetContentItemDataStream: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<byte[]>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (byte[])ApiClient.Deserialize(localVarResponse, typeof(byte[])));
            
        }

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager) 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>Task of byte[]</returns>
        public async System.Threading.Tasks.Task<byte[]> ContentGetContentItemDataStreamAsync(string contentType, string storeId, string relativeUrl)
        {
             ApiResponse<byte[]> localVarResponse = await ContentGetContentItemDataStreamAsyncWithHttpInfo(contentType, storeId, relativeUrl);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager) 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>Task of ApiResponse (byte[])</returns>
        public async System.Threading.Tasks.Task<ApiResponse<byte[]>> ContentGetContentItemDataStreamAsyncWithHttpInfo(string contentType, string storeId, string relativeUrl)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");
            // verify the required parameter 'relativeUrl' is set
            if (relativeUrl == null)
                throw new ApiException(400, "Missing required parameter 'relativeUrl' when calling VirtoCommerceContentApi->ContentGetContentItemDataStream");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (relativeUrl != null) localVarQueryParams.Add("relativeUrl", ApiClient.ParameterToString(relativeUrl)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentGetContentItemDataStream: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentGetContentItemDataStream: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<byte[]>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (byte[])ApiClient.Deserialize(localVarResponse, typeof(byte[])));
            
        }
        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>ContentStatistic</returns>
        public ContentStatistic ContentGetStoreContentStats(string storeId)
        {
             ApiResponse<ContentStatistic> localVarResponse = ContentGetStoreContentStatsWithHttpInfo(storeId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of ContentStatistic</returns>
        public ApiResponse<ContentStatistic> ContentGetStoreContentStatsWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentGetStoreContentStats");

            var localVarPath = "/api/content/{storeId}/stats";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentGetStoreContentStats: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentGetStoreContentStats: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ContentStatistic>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ContentStatistic)ApiClient.Deserialize(localVarResponse, typeof(ContentStatistic)));
            
        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>Task of ContentStatistic</returns>
        public async System.Threading.Tasks.Task<ContentStatistic> ContentGetStoreContentStatsAsync(string storeId)
        {
             ApiResponse<ContentStatistic> localVarResponse = await ContentGetStoreContentStatsAsyncWithHttpInfo(storeId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse (ContentStatistic)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<ContentStatistic>> ContentGetStoreContentStatsAsyncWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentGetStoreContentStats");

            var localVarPath = "/api/content/{storeId}/stats";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentGetStoreContentStats: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentGetStoreContentStats: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<ContentStatistic>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (ContentStatistic)ApiClient.Deserialize(localVarResponse, typeof(ContentStatistic)));
            
        }
        /// <summary>
        /// Rename or move content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns></returns>
        public void ContentMoveContent(string contentType, string storeId, string oldUrl, string newUrl)
        {
             ContentMoveContentWithHttpInfo(contentType, storeId, oldUrl, newUrl);
        }

        /// <summary>
        /// Rename or move content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> ContentMoveContentWithHttpInfo(string contentType, string storeId, string oldUrl, string newUrl)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'oldUrl' is set
            if (oldUrl == null)
                throw new ApiException(400, "Missing required parameter 'oldUrl' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'newUrl' is set
            if (newUrl == null)
                throw new ApiException(400, "Missing required parameter 'newUrl' when calling VirtoCommerceContentApi->ContentMoveContent");

            var localVarPath = "/api/content/{contentType}/{storeId}/move";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (oldUrl != null) localVarQueryParams.Add("oldUrl", ApiClient.ParameterToString(oldUrl)); // query parameter
            if (newUrl != null) localVarQueryParams.Add("newUrl", ApiClient.ParameterToString(newUrl)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentMoveContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentMoveContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Rename or move content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task ContentMoveContentAsync(string contentType, string storeId, string oldUrl, string newUrl)
        {
             await ContentMoveContentAsyncWithHttpInfo(contentType, storeId, oldUrl, newUrl);

        }

        /// <summary>
        /// Rename or move content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> ContentMoveContentAsyncWithHttpInfo(string contentType, string storeId, string oldUrl, string newUrl)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'oldUrl' is set
            if (oldUrl == null)
                throw new ApiException(400, "Missing required parameter 'oldUrl' when calling VirtoCommerceContentApi->ContentMoveContent");
            // verify the required parameter 'newUrl' is set
            if (newUrl == null)
                throw new ApiException(400, "Missing required parameter 'newUrl' when calling VirtoCommerceContentApi->ContentMoveContent");

            var localVarPath = "/api/content/{contentType}/{storeId}/move";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (oldUrl != null) localVarQueryParams.Add("oldUrl", ApiClient.ParameterToString(oldUrl)); // query parameter
            if (newUrl != null) localVarQueryParams.Add("newUrl", ApiClient.ParameterToString(newUrl)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentMoveContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentMoveContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Search content items in specified folder and using search keyword 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>List&lt;ContentItem&gt;</returns>
        public List<ContentItem> ContentSearchContent(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
             ApiResponse<List<ContentItem>> localVarResponse = ContentSearchContentWithHttpInfo(contentType, storeId, folderUrl, keyword);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Search content items in specified folder and using search keyword 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>ApiResponse of List&lt;ContentItem&gt;</returns>
        public ApiResponse<List<ContentItem>> ContentSearchContentWithHttpInfo(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentSearchContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentSearchContent");

            var localVarPath = "/api/content/{contentType}/{storeId}/search";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folderUrl != null) localVarQueryParams.Add("folderUrl", ApiClient.ParameterToString(folderUrl)); // query parameter
            if (keyword != null) localVarQueryParams.Add("keyword", ApiClient.ParameterToString(keyword)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentSearchContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentSearchContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ContentItem>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ContentItem>)ApiClient.Deserialize(localVarResponse, typeof(List<ContentItem>)));
            
        }

        /// <summary>
        /// Search content items in specified folder and using search keyword 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>Task of List&lt;ContentItem&gt;</returns>
        public async System.Threading.Tasks.Task<List<ContentItem>> ContentSearchContentAsync(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
             ApiResponse<List<ContentItem>> localVarResponse = await ContentSearchContentAsyncWithHttpInfo(contentType, storeId, folderUrl, keyword);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Search content items in specified folder and using search keyword 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched (optional)</param>
        /// <param name="keyword">search keyword (optional)</param>
        /// <returns>Task of ApiResponse (List&lt;ContentItem&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<ContentItem>>> ContentSearchContentAsyncWithHttpInfo(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentSearchContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentSearchContent");

            var localVarPath = "/api/content/{contentType}/{storeId}/search";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folderUrl != null) localVarQueryParams.Add("folderUrl", ApiClient.ParameterToString(folderUrl)); // query parameter
            if (keyword != null) localVarQueryParams.Add("keyword", ApiClient.ParameterToString(keyword)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentSearchContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentSearchContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ContentItem>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ContentItem>)ApiClient.Deserialize(localVarResponse, typeof(List<ContentItem>)));
            
        }
        /// <summary>
        /// Unpack contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        public void ContentUnpack(string contentType, string storeId, string archivePath, string destPath)
        {
             ContentUnpackWithHttpInfo(contentType, storeId, archivePath, destPath);
        }

        /// <summary>
        /// Unpack contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> ContentUnpackWithHttpInfo(string contentType, string storeId, string archivePath, string destPath)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'archivePath' is set
            if (archivePath == null)
                throw new ApiException(400, "Missing required parameter 'archivePath' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'destPath' is set
            if (destPath == null)
                throw new ApiException(400, "Missing required parameter 'destPath' when calling VirtoCommerceContentApi->ContentUnpack");

            var localVarPath = "/api/content/{contentType}/{storeId}/unpack";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (archivePath != null) localVarQueryParams.Add("archivePath", ApiClient.ParameterToString(archivePath)); // query parameter
            if (destPath != null) localVarQueryParams.Add("destPath", ApiClient.ParameterToString(destPath)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentUnpack: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentUnpack: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Unpack contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task ContentUnpackAsync(string contentType, string storeId, string archivePath, string destPath)
        {
             await ContentUnpackAsyncWithHttpInfo(contentType, storeId, archivePath, destPath);

        }

        /// <summary>
        /// Unpack contents 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> ContentUnpackAsyncWithHttpInfo(string contentType, string storeId, string archivePath, string destPath)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'archivePath' is set
            if (archivePath == null)
                throw new ApiException(400, "Missing required parameter 'archivePath' when calling VirtoCommerceContentApi->ContentUnpack");
            // verify the required parameter 'destPath' is set
            if (destPath == null)
                throw new ApiException(400, "Missing required parameter 'destPath' when calling VirtoCommerceContentApi->ContentUnpack");

            var localVarPath = "/api/content/{contentType}/{storeId}/unpack";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (archivePath != null) localVarQueryParams.Add("archivePath", ApiClient.ParameterToString(archivePath)); // query parameter
            if (destPath != null) localVarQueryParams.Add("destPath", ApiClient.ParameterToString(destPath)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentUnpack: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentUnpack: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>List&lt;ContentItem&gt;</returns>
        public List<ContentItem> ContentUploadContent(string contentType, string storeId, string folderUrl, string url = null)
        {
             ApiResponse<List<ContentItem>> localVarResponse = ContentUploadContentWithHttpInfo(contentType, storeId, folderUrl, url);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>ApiResponse of List&lt;ContentItem&gt;</returns>
        public ApiResponse<List<ContentItem>> ContentUploadContentWithHttpInfo(string contentType, string storeId, string folderUrl, string url = null)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentUploadContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentUploadContent");
            // verify the required parameter 'folderUrl' is set
            if (folderUrl == null)
                throw new ApiException(400, "Missing required parameter 'folderUrl' when calling VirtoCommerceContentApi->ContentUploadContent");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folderUrl != null) localVarQueryParams.Add("folderUrl", ApiClient.ParameterToString(folderUrl)); // query parameter
            if (url != null) localVarQueryParams.Add("url", ApiClient.ParameterToString(url)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentUploadContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentUploadContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ContentItem>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ContentItem>)ApiClient.Deserialize(localVarResponse, typeof(List<ContentItem>)));
            
        }

        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>Task of List&lt;ContentItem&gt;</returns>
        public async System.Threading.Tasks.Task<List<ContentItem>> ContentUploadContentAsync(string contentType, string storeId, string folderUrl, string url = null)
        {
             ApiResponse<List<ContentItem>> localVarResponse = await ContentUploadContentAsyncWithHttpInfo(contentType, storeId, folderUrl, url);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data (optional)</param>
        /// <returns>Task of ApiResponse (List&lt;ContentItem&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<ContentItem>>> ContentUploadContentAsyncWithHttpInfo(string contentType, string storeId, string folderUrl, string url = null)
        {
            // verify the required parameter 'contentType' is set
            if (contentType == null)
                throw new ApiException(400, "Missing required parameter 'contentType' when calling VirtoCommerceContentApi->ContentUploadContent");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->ContentUploadContent");
            // verify the required parameter 'folderUrl' is set
            if (folderUrl == null)
                throw new ApiException(400, "Missing required parameter 'folderUrl' when calling VirtoCommerceContentApi->ContentUploadContent");

            var localVarPath = "/api/content/{contentType}/{storeId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (contentType != null) localVarPathParams.Add("contentType", ApiClient.ParameterToString(contentType)); // path parameter
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (folderUrl != null) localVarQueryParams.Add("folderUrl", ApiClient.ParameterToString(folderUrl)); // query parameter
            if (url != null) localVarQueryParams.Add("url", ApiClient.ParameterToString(url)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling ContentUploadContent: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling ContentUploadContent: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<ContentItem>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<ContentItem>)ApiClient.Deserialize(localVarResponse, typeof(List<ContentItem>)));
            
        }
        /// <summary>
        /// Checking name of menu link list Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>bool?</returns>
        public bool? MenuCheckName(string storeId, string name, string language = null, string id = null)
        {
             ApiResponse<bool?> localVarResponse = MenuCheckNameWithHttpInfo(storeId, name, language, id);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Checking name of menu link list Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>ApiResponse of bool?</returns>
        public ApiResponse<bool?> MenuCheckNameWithHttpInfo(string storeId, string name, string language = null, string id = null)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuCheckName");
            // verify the required parameter 'name' is set
            if (name == null)
                throw new ApiException(400, "Missing required parameter 'name' when calling VirtoCommerceContentApi->MenuCheckName");

            var localVarPath = "/api/cms/{storeId}/menu/checkname";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (name != null) localVarQueryParams.Add("name", ApiClient.ParameterToString(name)); // query parameter
            if (language != null) localVarQueryParams.Add("language", ApiClient.ParameterToString(language)); // query parameter
            if (id != null) localVarQueryParams.Add("id", ApiClient.ParameterToString(id)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuCheckName: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuCheckName: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<bool?>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (bool?)ApiClient.Deserialize(localVarResponse, typeof(bool?)));
            
        }

        /// <summary>
        /// Checking name of menu link list Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>Task of bool?</returns>
        public async System.Threading.Tasks.Task<bool?> MenuCheckNameAsync(string storeId, string name, string language = null, string id = null)
        {
             ApiResponse<bool?> localVarResponse = await MenuCheckNameAsyncWithHttpInfo(storeId, name, language, id);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Checking name of menu link list Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list (optional)</param>
        /// <param name="id">Menu link list id (optional)</param>
        /// <returns>Task of ApiResponse (bool?)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<bool?>> MenuCheckNameAsyncWithHttpInfo(string storeId, string name, string language = null, string id = null)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuCheckName");
            // verify the required parameter 'name' is set
            if (name == null)
                throw new ApiException(400, "Missing required parameter 'name' when calling VirtoCommerceContentApi->MenuCheckName");

            var localVarPath = "/api/cms/{storeId}/menu/checkname";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (name != null) localVarQueryParams.Add("name", ApiClient.ParameterToString(name)); // query parameter
            if (language != null) localVarQueryParams.Add("language", ApiClient.ParameterToString(language)); // query parameter
            if (id != null) localVarQueryParams.Add("id", ApiClient.ParameterToString(id)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuCheckName: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuCheckName: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<bool?>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (bool?)ApiClient.Deserialize(localVarResponse, typeof(bool?)));
            
        }
        /// <summary>
        /// Delete menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public void MenuDelete(List<string> listIds, string storeId)
        {
             MenuDeleteWithHttpInfo(listIds, storeId);
        }

        /// <summary>
        /// Delete menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> MenuDeleteWithHttpInfo(List<string> listIds, string storeId)
        {
            // verify the required parameter 'listIds' is set
            if (listIds == null)
                throw new ApiException(400, "Missing required parameter 'listIds' when calling VirtoCommerceContentApi->MenuDelete");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuDelete");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (listIds != null) localVarQueryParams.Add("listIds", ApiClient.ParameterToString(listIds)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuDelete: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuDelete: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Delete menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task MenuDeleteAsync(List<string> listIds, string storeId)
        {
             await MenuDeleteAsyncWithHttpInfo(listIds, storeId);

        }

        /// <summary>
        /// Delete menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="listIds">Menu link list id</param>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> MenuDeleteAsyncWithHttpInfo(List<string> listIds, string storeId)
        {
            // verify the required parameter 'listIds' is set
            if (listIds == null)
                throw new ApiException(400, "Missing required parameter 'listIds' when calling VirtoCommerceContentApi->MenuDelete");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuDelete");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (listIds != null) localVarQueryParams.Add("listIds", ApiClient.ParameterToString(listIds)); // query parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuDelete: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuDelete: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
        /// <summary>
        /// Get menu link list by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>MenuLinkList</returns>
        public MenuLinkList MenuGetList(string storeId, string listId)
        {
             ApiResponse<MenuLinkList> localVarResponse = MenuGetListWithHttpInfo(storeId, listId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get menu link list by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>ApiResponse of MenuLinkList</returns>
        public ApiResponse<MenuLinkList> MenuGetListWithHttpInfo(string storeId, string listId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuGetList");
            // verify the required parameter 'listId' is set
            if (listId == null)
                throw new ApiException(400, "Missing required parameter 'listId' when calling VirtoCommerceContentApi->MenuGetList");

            var localVarPath = "/api/cms/{storeId}/menu/{listId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (listId != null) localVarPathParams.Add("listId", ApiClient.ParameterToString(listId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuGetList: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuGetList: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<MenuLinkList>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (MenuLinkList)ApiClient.Deserialize(localVarResponse, typeof(MenuLinkList)));
            
        }

        /// <summary>
        /// Get menu link list by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>Task of MenuLinkList</returns>
        public async System.Threading.Tasks.Task<MenuLinkList> MenuGetListAsync(string storeId, string listId)
        {
             ApiResponse<MenuLinkList> localVarResponse = await MenuGetListAsyncWithHttpInfo(storeId, listId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get menu link list by id 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <param name="listId">List id</param>
        /// <returns>Task of ApiResponse (MenuLinkList)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MenuLinkList>> MenuGetListAsyncWithHttpInfo(string storeId, string listId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuGetList");
            // verify the required parameter 'listId' is set
            if (listId == null)
                throw new ApiException(400, "Missing required parameter 'listId' when calling VirtoCommerceContentApi->MenuGetList");

            var localVarPath = "/api/cms/{storeId}/menu/{listId}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (listId != null) localVarPathParams.Add("listId", ApiClient.ParameterToString(listId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuGetList: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuGetList: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<MenuLinkList>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (MenuLinkList)ApiClient.Deserialize(localVarResponse, typeof(MenuLinkList)));
            
        }
        /// <summary>
        /// Get menu link lists 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>List&lt;MenuLinkList&gt;</returns>
        public List<MenuLinkList> MenuGetLists(string storeId)
        {
             ApiResponse<List<MenuLinkList>> localVarResponse = MenuGetListsWithHttpInfo(storeId);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get menu link lists 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>ApiResponse of List&lt;MenuLinkList&gt;</returns>
        public ApiResponse<List<MenuLinkList>> MenuGetListsWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuGetLists");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuGetLists: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuGetLists: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<MenuLinkList>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<MenuLinkList>)ApiClient.Deserialize(localVarResponse, typeof(List<MenuLinkList>)));
            
        }

        /// <summary>
        /// Get menu link lists 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of List&lt;MenuLinkList&gt;</returns>
        public async System.Threading.Tasks.Task<List<MenuLinkList>> MenuGetListsAsync(string storeId)
        {
             ApiResponse<List<MenuLinkList>> localVarResponse = await MenuGetListsAsyncWithHttpInfo(storeId);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get menu link lists 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="storeId">Store id</param>
        /// <returns>Task of ApiResponse (List&lt;MenuLinkList&gt;)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<List<MenuLinkList>>> MenuGetListsAsyncWithHttpInfo(string storeId)
        {
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuGetLists");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml"
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuGetLists: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuGetLists: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            return new ApiResponse<List<MenuLinkList>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<MenuLinkList>)ApiClient.Deserialize(localVarResponse, typeof(List<MenuLinkList>)));
            
        }
        /// <summary>
        /// Update menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public void MenuUpdate(MenuLinkList list, string storeId)
        {
             MenuUpdateWithHttpInfo(list, storeId);
        }

        /// <summary>
        /// Update menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>ApiResponse of Object(void)</returns>
        public ApiResponse<object> MenuUpdateWithHttpInfo(MenuLinkList list, string storeId)
        {
            // verify the required parameter 'list' is set
            if (list == null)
                throw new ApiException(400, "Missing required parameter 'list' when calling VirtoCommerceContentApi->MenuUpdate");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuUpdate");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (list.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(list); // http body (model) parameter
            }
            else
            {
                localVarPostBody = list; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuUpdate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuUpdate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }

        /// <summary>
        /// Update menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>Task of void</returns>
        public async System.Threading.Tasks.Task MenuUpdateAsync(MenuLinkList list, string storeId)
        {
             await MenuUpdateAsyncWithHttpInfo(list, storeId);

        }

        /// <summary>
        /// Update menu link list 
        /// </summary>
        /// <exception cref="VirtoCommerce.ContentModule.Client.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="list">Menu link list</param>
        /// <param name="storeId"></param>
        /// <returns>Task of ApiResponse</returns>
        public async System.Threading.Tasks.Task<ApiResponse<object>> MenuUpdateAsyncWithHttpInfo(MenuLinkList list, string storeId)
        {
            // verify the required parameter 'list' is set
            if (list == null)
                throw new ApiException(400, "Missing required parameter 'list' when calling VirtoCommerceContentApi->MenuUpdate");
            // verify the required parameter 'storeId' is set
            if (storeId == null)
                throw new ApiException(400, "Missing required parameter 'storeId' when calling VirtoCommerceContentApi->MenuUpdate");

            var localVarPath = "/api/cms/{storeId}/menu";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
                "application/json", 
                "text/json", 
                "application/xml", 
                "text/xml", 
                "application/x-www-form-urlencoded"
            };
            string localVarHttpContentType = ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
            };
            string localVarHttpHeaderAccept = ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (storeId != null) localVarPathParams.Add("storeId", ApiClient.ParameterToString(storeId)); // path parameter
            if (list.GetType() != typeof(byte[]))
            {
                localVarPostBody = ApiClient.Serialize(list); // http body (model) parameter
            }
            else
            {
                localVarPostBody = list; // byte array
            }


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse)await ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int)localVarResponse.StatusCode;

            if (localVarStatusCode >= 400 && (localVarStatusCode != 404 || Configuration.ThrowExceptionWhenStatusCodeIs404))
                throw new ApiException(localVarStatusCode, "Error calling MenuUpdate: " + localVarResponse.Content, localVarResponse.Content);
            else if (localVarStatusCode == 0)
                throw new ApiException(localVarStatusCode, "Error calling MenuUpdate: " + localVarResponse.ErrorMessage, localVarResponse.ErrorMessage);

            
            return new ApiResponse<object>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                null);
        }
    }
}
