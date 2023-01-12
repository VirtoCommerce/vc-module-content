namespace VirtoCommerce.ContentModule.Core
{
    public static class ContentConstants
    {
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

        public static class ContentTypes
        {
            public const string Blogs = "blogs";
            public const string Pages = "pages";
            public const string Themes = "themes";
        }

        public const string DefaultTheme = "default";

    }
}
