namespace DigitalDocumentPlatform.Common.MemoryCache
{
    public static class CacheKeys
    {
        // Status
        public const string AllStatuses = "master:statuses:all";
        public static string StatusesByType(string type) => $"master:statuses:type:{type}";
        public static string StatusById(int id) => $"master:statuses:{id}";

        // Priority
        public const string AllPriorities = "master:priorities:all";
        public static string PrioritiesByType(string type) => $"master:priorities:type:{type}";
        public static string PriorityById(int id) => $"master:priorities:{id}";

        // Category
        public const string AllCategories = "master:categories:all";
        public const string ActiveCategories = "master:categories:active";
        public static string CategoryById(int id) => $"master:categories:{id}";

        // ProjectType
        public const string AllProjectTypes = "master:projecttypes:all";
        public static string ProjectTypeById(int id) => $"master:projecttypes:{id}";

        // Team
        public const string AllTeams = "master:teams:all";
        public const string ActiveTeams = "master:teams:active";
        public static string TeamById(int id) => $"master:teams:{id}";

        // Role
        public const string AllRoles = "master:roles:all";
        public static string RoleById(int id) => $"master:roles:{id}";

        // Tag
        public const string PopularTags = "master:tags:popular";
        public const string AllActiveTags = "master:tags:active";

        // Permission
        public const string AllPermissions = "master:permissions:all";
        public static string PermissionById(int id) => $"master:permissions:{id}";

        // Cache duration
        public static readonly TimeSpan MasterDataExpiry = TimeSpan.FromDays(1);
    }
}