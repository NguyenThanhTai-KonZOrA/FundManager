namespace FundManager.Common.Constants
{
    public class CommonConstants
    {
        #region Created by system
        public const string SystemUser = "System";
        #endregion
        public const string UnknowUser = "Anonymous";
        public const int UnknowUserId = 0;
        public const string AdminUserName = "admin";
        public const string AdminPassword = "admin@123";
        public const string AdminRole = "Administrator";
        public const string UserRole = "user";
        public const int DefaultMemberId = 1;
        public const string DefaultConnection = "DefaultConnection";

        // Application Settings Keys
        public const string CacheExpirationMinutesKey = "CacheExpiration";
        public const string EnableCheckAdministratorKey = "EnableCheckAdministrator";
        public const string MaxDataLoadOnPageKey = "MaxDataLoadOnPage";
        public const string EnableSendEmailKey = "IsSentEmail";
        public const string ListEmailCCKey = "ListEmailCC";
        public const string ListEmailToKey = "ListEmailTo";

        // Permissions constants
        public const string CanViewDashboard = "CAN_VIEW_DASHBOARD";

        // Application Settings Data Types
        public const string String = "String";
        public const string Integer = "Integer";
        public const string Decimal = "Decimal";
        public const string Boolean = "Boolean";
        public const string Json = "Json";

        // Language
        public const string DefaultLanguage = "en";
    }
}