using System;
using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ContentModule.Core;

public static class ContentConstants
{
    public const string AnyIndexValue = "any";

    public static class Security
    {
        public static class Permissions
        {
            public const string Read = "content:read",
                Access = "content:access",
                Create = "content:create",
                Delete = "content:delete",
                Update = "content:update";

            public static string[] AllPermissions = { Read, Access, Create, Delete, Update };
        }
    }

    public static class Settings
    {
        public static class Search
        {
            public static SettingDescriptor IndexationDateContent { get; } = new()
            {
                Name = $"VirtoCommerce.Search.IndexingJobs.IndexationDate.{nameof(ContentFile)}",
                GroupName = "Content|Search",
                ValueType = SettingValueType.DateTime,
                DefaultValue = default(DateTime),
            };
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                yield return Search.IndexationDateContent;
            }
        }
    }

    public static class ContentTypes
    {
        public const string Blogs = "blogs";
        public const string Pages = "pages";
        public const string Themes = "themes";
    }

    public const string DefaultTheme = "default";
}
